import { Injectable, ErrorHandler } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from './auth.service';

@Injectable({
    providedIn: 'root'
})
export class GlobalErrorHandler implements ErrorHandler {
    constructor(
        private snackBar: MatSnackBar,
        private authService: AuthService
    ) {}

    handleError(error: Error | HttpErrorResponse): void {
        let errorMessage: string;

        if (error instanceof HttpErrorResponse) {
            // Server or connection error
            if (!navigator.onLine) {
                errorMessage = 'No Internet Connection';
            } else {
                // Handle different HTTP status codes
                switch (error.status) {
                    case 400:
                        errorMessage = this.getValidationErrorMessage(error);
                        break;
                    case 401:
                        errorMessage = 'Unauthorized access';
                        this.authService.logout();
                        break;
                    case 403:
                        errorMessage = 'Access forbidden';
                        break;
                    case 404:
                        errorMessage = 'Resource not found';
                        break;
                    case 500:
                        errorMessage = 'Internal server error';
                        break;
                    default:
                        errorMessage = `Error: ${error.message}`;
                }
            }
        } else {
            // Client Error
            errorMessage = error.message ? error.message : error.toString();
        }

        // Log the error
        console.error('An error occurred:', error);

        // Show error message to user
        this.snackBar.open(errorMessage, 'Close', {
            duration: 5000,
            horizontalPosition: 'center',
            verticalPosition: 'bottom',
            panelClass: ['error-snackbar']
        });
    }

    private getValidationErrorMessage(error: HttpErrorResponse): string {
        if (error.error?.errors) {
            // Handle validation errors from the server
            const validationErrors = error.error.errors;
            return Object.values(validationErrors)
                .flat()
                .join('\n');
        }
        return error.error?.message || 'Bad request';
    }
} 