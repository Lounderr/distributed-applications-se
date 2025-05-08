import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class OnlineUsersService {
    private apiUrl = `${environment.apiUrl}/users/online`;

    constructor(private http: HttpClient) {}

    getOnlineUsers(): Observable<string[]> {
        return this.http.get<{status: number, data: string[]}>(this.apiUrl).pipe(
            map(response => response.data)
        );
    }
} 