import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LoginDto } from '../../models/auth/login.dto';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        MatCardModule,
        MatFormFieldModule,
        MatInputModule,
        MatButtonModule,
        MatIconModule,
        RouterLink,
        MatProgressSpinnerModule
    ],
    template: `
        <div class="login-container">
            <mat-card>
                <mat-card-header>
                    <mat-card-title>Login</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                    <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
                        <mat-form-field appearance="outline">
                            <mat-label>Email</mat-label>
                            <input matInput formControlName="email" type="email" placeholder="Enter your email">
                            <mat-error *ngIf="loginForm.get('email')?.hasError('required')">
                                Email is required
                            </mat-error>
                            <mat-error *ngIf="loginForm.get('email')?.hasError('email')">
                                Please enter a valid email
                            </mat-error>
                        </mat-form-field>

                        <mat-form-field appearance="outline">
                            <mat-label>Password</mat-label>
                            <input matInput formControlName="password" [type]="hidePassword ? 'password' : 'text'">
                            <button mat-icon-button matSuffix (click)="hidePassword = !hidePassword" type="button">
                                <mat-icon>{{hidePassword ? 'visibility_off' : 'visibility'}}</mat-icon>
                            </button>
                            <mat-error *ngIf="loginForm.get('password')?.hasError('required')">
                                Password is required
                            </mat-error>
                        </mat-form-field>

                        <button mat-raised-button color="primary" type="submit" [disabled]="loginForm.invalid || isLoading">
                            <mat-spinner diameter="20" *ngIf="isLoading"></mat-spinner>
                            <span *ngIf="!isLoading">Login</span>
                        </button>

                        <div class="register-link">
                            Don't have an account? <a routerLink="/register">Register here</a>
                        </div>
                    </form>
                </mat-card-content>
            </mat-card>
        </div>
    `,
    styles: [`
        .login-container {
            height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
            background-color: #f5f5f5;
        }

        mat-card {
            width: 100%;
            max-width: 400px;
            padding: 20px;
        }

        form {
            display: flex;
            flex-direction: column;
            gap: 16px;
        }

        mat-form-field {
            width: 100%;
        }

        button[type="submit"] {
            width: 100%;
            height: 40px;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .register-link {
            text-align: center;
            margin-top: 16px;
        }

        mat-spinner {
            margin: 0 auto;
        }
    `]
})
export class LoginComponent {
    loginForm: FormGroup;
    hidePassword = true;
    isLoading = false;

    constructor(
        private fb: FormBuilder,
        private authService: AuthService,
        private router: Router,
        private snackBar: MatSnackBar
    ) {
        this.loginForm = this.fb.group({
            email: ['', [Validators.required, Validators.email]],
            password: ['', Validators.required]
        });
    }

    onSubmit(): void {
        if (this.loginForm.valid) {
            this.isLoading = true;
            const loginDto: LoginDto = {
                email: this.loginForm.value.email,
                password: this.loginForm.value.password
            };

            this.authService.login(loginDto).subscribe({
                next: () => {
                    this.router.navigate(['/']);
                },
                error: (error) => {
                    this.isLoading = false;
                    this.snackBar.open(error.message || 'Login failed. Please check your credentials.', 'Close', {
                        duration: 3000
                    });
                },
                complete: () => {
                    this.isLoading = false;
                }
            });
        }
    }
} 