import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { apiBaseUrl } from '../config/api-url';
import { AuthService, AuthUser } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting()
      ]
    });

    service = TestBed.inject(AuthService);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('reuses the completed unauthenticated session check', () => {
    let firstResult: boolean | undefined;
    let secondResult: boolean | undefined;

    service.ensureAuthenticated().subscribe(result => firstResult = result);

    const request = httpTesting.expectOne(`${apiBaseUrl}/auth/me`);
    request.flush(null, { status: 401, statusText: 'Unauthorized' });

    service.ensureAuthenticated().subscribe(result => secondResult = result);

    httpTesting.expectNone(`${apiBaseUrl}/auth/me`);
    expect(firstResult).toBe(false);
    expect(secondResult).toBe(false);
  });

  it('reuses the authenticated user loaded from the server', () => {
    const user: AuthUser = {
      userId: 1,
      username: 'admin',
      fullName: 'Admin User',
      role: 'Admin'
    };

    service.ensureAuthenticated().subscribe();

    const request = httpTesting.expectOne(`${apiBaseUrl}/auth/me`);
    request.flush(user);

    let result: boolean | undefined;
    service.ensureAuthenticated().subscribe(value => result = value);

    httpTesting.expectNone(`${apiBaseUrl}/auth/me`);
    expect(result).toBe(true);
  });

  it('uses the dedicated endpoint to start a demo session', () => {
    let user: AuthUser | undefined;

    service.loginDemo().subscribe(result => user = result);

    const request = httpTesting.expectOne(`${apiBaseUrl}/auth/demo-login`);
    expect(request.request.method).toBe('POST');
    request.flush({
      expiresAtUtc: '2026-07-20T12:00:00Z',
      user: {
        userId: 15,
        username: 'demo',
        fullName: 'Demo User',
        role: 'Demo'
      }
    });

    expect(user?.role).toBe('Demo');
    expect(service.isDemoUser()).toBe(true);
  });
});
