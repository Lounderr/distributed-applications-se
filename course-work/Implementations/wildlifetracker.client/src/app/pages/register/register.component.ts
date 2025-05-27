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
import { RegisterDto } from '../../models/auth/register.dto';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

@Component({
    selector: 'app-register',
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
        MatDatepickerModule,
        MatNativeDateModule
    ],
    template: `
        <div class="register-container">
            <mat-card>
                <mat-card-header>
                    <mat-card-title>Register</mat-card-title>
                </mat-card-header>
                <mat-card-content>
                    <form [formGroup]="registerForm" (ngSubmit)="onSubmit()">
                        <mat-form-field appearance="outline">
                            <mat-label>First Name</mat-label>
                            <input matInput formControlName="firstName" placeholder="Enter your first name">
                            <mat-error *ngIf="registerForm.get('firstName')?.hasError('required')">
                                First name is required
                            </mat-error>
                        </mat-form-field>

                        <mat-form-field appearance="outline">
                            <mat-label>Last Name</mat-label>
                            <input matInput formControlName="lastName" placeholder="Enter your last name">
                            <mat-error *ngIf="registerForm.get('lastName')?.hasError('required')">
                                Last name is required
                            </mat-error>
                        </mat-form-field>

                        <mat-form-field appearance="outline">
                            <mat-label>Email</mat-label>
                            <input matInput formControlName="email" type="email" placeholder="Enter your email">
                            <mat-error *ngIf="registerForm.get('email')?.hasError('required')">
                                Email is required
                            </mat-error>
                            <mat-error *ngIf="registerForm.get('email')?.hasError('email')">
                                Please enter a valid email
                            </mat-error>
                        </mat-form-field>

                        <mat-form-field appearance="outline">
                            <mat-label>Phone Number</mat-label>
                            <input matInput formControlName="phoneNumber" placeholder="Enter your phone number">
                            <mat-error *ngIf="registerForm.get('phoneNumber')?.hasError('required')">
                                Phone number is required
                            </mat-error>
                        </mat-form-field>

                        <mat-form-field appearance="outline">
                            <mat-label>Date of Birth</mat-label>
                            <input matInput [matDatepicker]="picker" formControlName="dateOfBirth">
                            <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
                            <mat-datepicker #picker></mat-datepicker>
                            <mat-error *ngIf="registerForm.get('dateOfBirth')?.hasError('required')">
                                Date of birth is required
                            </mat-error>
                        </mat-form-field>

                        <mat-form-field appearance="outline">
                            <mat-label>Password</mat-label>
                            <input matInput formControlName="password" [type]="hidePassword ? 'password' : 'text'">
                            <button mat-icon-button matSuffix (click)="hidePassword = !hidePassword" type="button">
                                <mat-icon>{{hidePassword ? 'visibility_off' : 'visibility'}}</mat-icon>
                            </button>
                            <mat-error *ngIf="registerForm.get('password')?.hasError('required')">
                                Password is required
                            </mat-error>
                            <mat-error *ngIf="registerForm.get('password')?.hasError('minlength')">
                                Password must be at least 6 characters
                            </mat-error>
                        </mat-form-field>

                        <mat-form-field appearance="outline">
                            <mat-label>Confirm Password</mat-label>
                            <input matInput formControlName="confirmPassword" [type]="hidePassword ? 'password' : 'text'">
                            <mat-error *ngIf="registerForm.get('confirmPassword')?.hasError('required')">
                                Please confirm your password
                            </mat-error>
                            <mat-error *ngIf="registerForm.get('confirmPassword')?.hasError('passwordMismatch')">
                                Passwords do not match
                            </mat-error>
                        </mat-form-field>

                        <button mat-raised-button color="primary" type="submit" [disabled]="registerForm.invalid">
                            Register
                        </button>

                        <div class="login-link">
                            Already have an account? <a routerLink="/login">Login here</a>
                        </div>
                    </form>
                </mat-card-content>
            </mat-card>
        </div>
    `,
    styles: [`
        .register-container {
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
        }

        .login-link {
            text-align: center;
            margin-top: 16px;
        }
    `]
})
export class RegisterComponent {
    registerForm: FormGroup;
    hidePassword = true;

    constructor(
        private fb: FormBuilder,
        private authService: AuthService,
        private router: Router,
        private snackBar: MatSnackBar
    ) {
        this.registerForm = this.fb.group({
            firstName: ['', Validators.required],
            lastName: ['', Validators.required],
            email: ['', [Validators.required, Validators.email]],
            phoneNumber: ['', Validators.required],
            dateOfBirth: ['', Validators.required],
            password: ['', [Validators.required, Validators.minLength(6)]],
            confirmPassword: ['', [Validators.required]]
        }, { validators: this.passwordMatchValidator });
    }

    passwordMatchValidator(form: FormGroup) {
        const password = form.get('password')?.value;
        const confirmPassword = form.get('confirmPassword')?.value;
        return password === confirmPassword ? null : { passwordMismatch: true };
    }

    onSubmit(): void {
        if (this.registerForm.valid) {
            const registerDto: RegisterDto = {
                firstName: this.registerForm.value.firstName,
                lastName: this.registerForm.value.lastName,
                email: this.registerForm.value.email,
                phoneNumber: this.registerForm.value.phoneNumber,
                dateOfBirth: this.registerForm.value.dateOfBirth.toISOString().split('T')[0],
                password: this.registerForm.value.password
            };

            this.authService.register(registerDto).subscribe({
                next: () => {
                    this.snackBar.open('Registration successful! Please login.', 'Close', {
                        duration: 3000
                    });
                    this.router.navigate(['/login']);
                },
                error: (error) => {
                    this.snackBar.open(error.message || 'Registration failed', 'Close', {
                        duration: 3000
                    });
                }
            });
        }
    }
} 