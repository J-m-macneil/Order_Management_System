import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, catchError, finalize, map, of, tap } from 'rxjs';
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
  private sessionChecked = false;

  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) { }

  login(credentials: { usernameOrEmail: string; password: string }): Observable<AuthUser> {
    return this.http.post<LoginResponse>(`${apiBaseUrl}/auth/login`, credentials).pipe(
      tap(response => this.setCurrentUser(response.user)),
      map(response => response.user)
    );
  }

  logout(): Observable<void> {
    return this.http.post<void>(`${apiBaseUrl}/auth/logout`, {}).pipe(
      catchError(() => of(void 0)),
      finalize(() => this.clearSession())
    );
  }

  refresh(): Observable<void> {
    return this.http.post<LoginResponse>(`${apiBaseUrl}/auth/refresh`, {}).pipe(
      tap(response => this.setCurrentUser(response.user)),
      map(() => void 0)
    );
  }

  ensureAuthenticated(): Observable<boolean> {
    if (this.sessionChecked) {
      return of(this.isLoggedIn());
    }

    return this.loadCurrentUser().pipe(
      map(user => user !== null)
    );
  }

  loadCurrentUser(): Observable<AuthUser | null> {
    return this.http.get<AuthUser>(`${apiBaseUrl}/auth/me`).pipe(
      tap(user => this.setCurrentUser(user)),
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
    return !!userRole && roles.some(role =>
      role.toLowerCase() === userRole.toLowerCase()
    );
  }

  clearSession(): void {
    this.setCurrentUser(null);
  }

  private setCurrentUser(user: AuthUser | null): void {
    this.currentUserSubject.next(user);
    this.sessionChecked = true;
  }
}
