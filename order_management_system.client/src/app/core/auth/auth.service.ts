import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  setToken(token: string): void {
    localStorage.setItem('auth_token', token);
  }

  clearToken(): void {
    localStorage.removeItem('auth_token');
  }

  logout(): void {
    this.clearToken();
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getUserRole(): string | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return (
        payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
        payload['role'] ||
        null
      );
    } catch {
      return null;
    }
  }

  hasRole(...roles: string[]): boolean {
    const userRole = this.getUserRole();
    return !!userRole && roles.includes(userRole);
  }
}
