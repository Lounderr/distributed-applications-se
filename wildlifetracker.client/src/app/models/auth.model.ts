export interface LoginDto {
    email: string;
    password: string;
}

export interface RegisterDto {
    email: string;
    password: string;
    firstName: string;
    lastName: string;
    phoneNumber: string;
    city?: string;
    dateOfBirth: string;
}

export interface RefreshRequest {
    refreshToken: string;
}

export interface AuthResponse {
    accessToken: string;
    refreshToken: string;
} 