import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { environment } from '../../environments/environment';

export const authInterceptor: HttpInterceptorFn = (
    request: HttpRequest<unknown>,
    next: HttpHandlerFn
) => {
    const authService = inject(AuthService);
    const token = authService.getAccessToken();

    // Skip auth for login, register, and refresh endpoints
    if (request.url.includes('/api/v1/identity/login') ||
        request.url.includes('/api/v1/identity/register') ||
        request.url.includes('/api/v1/identity/refresh')) {
        return next(request);
    }

    // Add auth header if token exists
    if (token) {
        request = request.clone({
            setHeaders: {
                Authorization: `Bearer ${token}`
            }
        });
    }

    return next(request).pipe(
        catchError((error: HttpErrorResponse) => {
            if (error.status === 401 && !request.url.includes('/api/v1/identity/refresh')) {
                return authService.refreshToken().pipe(
                    switchMap(() => {
                        const newToken = authService.getAccessToken();
                        if (newToken) {
                            request = request.clone({
                                setHeaders: {
                                    Authorization: `Bearer ${newToken}`
                                }
                            });
                            return next(request);
                        }
                        return throwError(() => error);
                    }),
                    catchError((refreshError) => {
                        authService.logout();
                        return throwError(() => refreshError);
                    })
                );
            }
            return throwError(() => error);
        })
    );
}; 