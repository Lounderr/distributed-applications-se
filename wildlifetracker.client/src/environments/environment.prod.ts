export const environment = {
    production: true,
    apiUrl: '/api/v1', // In production, we'll use relative URLs
    authConfig: {
        tokenKey: 'auth_token',
        refreshTokenKey: 'refresh_token',
        tokenExpiryKey: 'token_expiry'
    }
}; 