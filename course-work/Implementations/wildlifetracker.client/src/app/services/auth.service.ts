import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, timer } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Router } from '@angular/router';
import { LoginDto, RegisterDto, RefreshRequest, AuthResponse } from '../models/auth.model';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private readonly TOKEN_KEY = 'access_token';
    private readonly REFRESH_TOKEN_KEY = 'refresh_token';
    private readonly REFRESH_INTERVAL = 15 * 60 * 1000; // 15 minutes in milliseconds

    private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());
    isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

    constructor(
        private http: HttpClient,
        private router: Router
    ) {
        this.setupAutoRefresh();
    }

    private setupAutoRefresh(): void {
        if (this.hasToken()) {
            // Start the refresh timer
            timer(this.REFRESH_INTERVAL, this.REFRESH_INTERVAL).subscribe(() => {
                if (this.hasToken()) {
                    this.refreshToken().subscribe();
                }
            });
        }
    }

    login(credentials: LoginDto): Observable<AuthResponse> {
        return this.http.post<AuthResponse>(`${environment.apiUrl}/identity/login`, credentials)
            .pipe(
                tap(response => this.handleAuthResponse(response))
            );
    }

    register(userData: RegisterDto): Observable<AuthResponse> {
        return this.http.post<AuthResponse>(`${environment.apiUrl}/identity/register`, userData)
            .pipe(
                tap(response => this.handleAuthResponse(response))
            );
    }

    refreshToken(): Observable<AuthResponse> {
        const refreshToken = localStorage.getItem(this.REFRESH_TOKEN_KEY);
        if (!refreshToken) {
            return new Observable(subscriber => {
                subscriber.error('No refresh token available');
                subscriber.complete();
            });
        }

        const request: RefreshRequest = { refreshToken };
        return this.http.post<AuthResponse>(`${environment.apiUrl}/identity/refresh`, request)
            .pipe(
                tap(response => this.handleAuthResponse(response))
            );
    }

    logout(): void {
        localStorage.removeItem(this.TOKEN_KEY);
        localStorage.removeItem(this.REFRESH_TOKEN_KEY);
        this.isAuthenticatedSubject.next(false);
        this.router.navigate(['/login']);
    }

    getAccessToken(): string | null {
        return localStorage.getItem(this.TOKEN_KEY);
    }

    hasToken(): boolean {
        return !!this.getAccessToken();
    }

    private handleAuthResponse(response: AuthResponse): void {
        localStorage.setItem(this.TOKEN_KEY, response.accessToken);
        localStorage.setItem(this.REFRESH_TOKEN_KEY, response.refreshToken);
        this.isAuthenticatedSubject.next(true);
    }
} 