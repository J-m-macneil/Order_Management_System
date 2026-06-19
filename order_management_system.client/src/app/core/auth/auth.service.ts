import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, catchError, map, of, tap } from 'rxjs';
import { apiBaseUrl } from '../config/api-url';

export interface AuthUser {
  userId: number;
  username: string;
  fullName: string;
  role: string;
}

interface LoginResponse {
  expiresAtUtc: string;
  user: AuthUser;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly currentUserSubject = new BehaviorSubject<AuthUser | null>(null);

  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) { }

  login(credentials: { usernameOrEmail: string; password: string }): Observable<AuthUser> {
    return this.http.post<LoginResponse>(`${apiBaseUrl}/auth/login`, credentials).pipe(
      tap(response => this.setCurrentUser(response.user)),
      map(response => response.user)
    );
  }

  logout(): void {
    this.http.post<void>(`${apiBaseUrl}/auth/logout`, {}).subscribe();
    this.setCurrentUser(null);
  }

  ensureAuthenticated(): Observable<boolean> {
    if (this.currentUserSubject.value) {
      return of(true);
    }

    return this.loadCurrentUser().pipe(
      map(user => !!user),
      catchError(() => of(false))
    );
  }

  loadCurrentUser(): Observable<AuthUser | null> {
    return this.http.get<AuthUser>(`${apiBaseUrl}/auth/me`).pipe(
      tap(user => this.setCurrentUser(user)),
      map(user => user),
      catchError(() => {
        this.setCurrentUser(null);
        return of(null);
      })
    );
  }

  isLoggedIn(): boolean {
    return !!this.currentUserSubject.value;
  }

  getUserRole(): string | null {
    return this.currentUserSubject.value?.role ?? null;
  }

  getUserFullName(): string | null {
    return this.currentUserSubject.value?.fullName ?? null;
  }

  getUsername(): string | null {
    return this.currentUserSubject.value?.username ?? null;
  }

  hasRole(...roles: string[]): boolean {
    const userRole = this.getUserRole();
    return !!userRole && roles.includes(userRole);
  }

  private setCurrentUser(user: AuthUser | null): void {
    this.currentUserSubject.next(user);
  }
}
