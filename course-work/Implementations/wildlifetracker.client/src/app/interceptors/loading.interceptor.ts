import { HttpInterceptorFn, HttpRequest, HttpHandlerFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { LoadingService } from '../services/loading.service';

export const loadingInterceptor: HttpInterceptorFn = (
    request: HttpRequest<unknown>,
    next: HttpHandlerFn
) => {
    const loadingService = inject(LoadingService);
    
    // Don't show loading for specific endpoints if needed
    if (request.url.includes('/api/v1/identity/refresh')) {
        return next(request);
    }

    loadingService.show();
    
    return next(request).pipe(
        finalize(() => {
            loadingService.hide();
        })
    );
}; 