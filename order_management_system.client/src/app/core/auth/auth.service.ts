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
    const payload = this.getTokenPayload();

    if (!payload) {
      return false;
    }

    if (!payload.exp) {
      return true;
    }

    return payload.exp * 1000 > Date.now();
  }

  private getTokenPayload(): any | null {
    const token = this.getToken();

    if (!token) {
      return null;
    }

    try {
      return JSON.parse(atob(token.split('.')[1]));
    } catch {
      return null;
    }
  }

  getUserRole(): string | null {
    const payload = this.getTokenPayload();

    return (
      payload?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
      payload?.role ||
      null
    );
  }

  getUserFullName(): string | null {
    const payload = this.getTokenPayload();

    return (
      payload?.fullName ||
      this.getUsername() ||
      null
    );
  }

  getUsername(): string | null {
    const payload = this.getTokenPayload();

    return (
      payload?.unique_name ||
      payload?.['unique_name'] ||
      payload?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ||
      null
    );
  }

  hasRole(...roles: string[]): boolean {
    const userRole = this.getUserRole();
    return !!userRole && roles.includes(userRole);
  }
}
