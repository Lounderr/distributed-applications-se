import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './app/interceptors/auth.interceptor';
import { loadingInterceptor } from './app/interceptors/loading.interceptor';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideRouter } from '@angular/router';
import { APP_ROUTES } from './app/app.routes';
import { ErrorHandler } from '@angular/core';
import { GlobalErrorHandler } from './app/services/error-handler.service';

bootstrapApplication(AppComponent, {
    providers: [
        provideRouter(APP_ROUTES),
        provideAnimations(),
        provideHttpClient(
            withInterceptors([authInterceptor, loadingInterceptor])
        ),
        { provide: ErrorHandler, useClass: GlobalErrorHandler }
    ]
}).catch(err => console.error(err));
