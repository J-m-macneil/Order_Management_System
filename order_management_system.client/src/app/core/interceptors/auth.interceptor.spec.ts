import {
  HTTP_INTERCEPTORS,
  HttpClient,
  HttpErrorResponse,
  HttpResponse,
  provideHttpClient,
  withInterceptorsFromDi
} from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { AuthInterceptor } from './auth.interceptor';

describe('AuthInterceptor', () => {
  let http: HttpClient;
  let httpTesting: HttpTestingController;
  let authService: Pick<AuthService, 'refresh' | 'clearSession'>;
  let router: Pick<Router, 'navigate'>;

  beforeEach(() => {
    authService = {
      refresh: vi.fn().mockReturnValue(of(void 0)),
      clearSession: vi.fn()
    };
    router = { navigate: vi.fn() };

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptorsFromDi()),
        provideHttpClientTesting(),
        {
          provide: HTTP_INTERCEPTORS,
          useClass: AuthInterceptor,
          multi: true
        },
        { provide: AuthService, useValue: authService },
        { provide: Router, useValue: router }
      ]
    });

    http = TestBed.inject(HttpClient);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('refreshes the session and retries an unauthorized request', () => {
    let response: HttpResponse<unknown> | undefined;

    http.get('/api/orders', { observe: 'response' })
      .subscribe(result => response = result);

    const initialRequest = httpTesting.expectOne('/api/orders');
    expect(initialRequest.request.withCredentials).toBe(true);
    initialRequest.flush(null, { status: 401, statusText: 'Unauthorized' });

    expect(authService.refresh).toHaveBeenCalledOnce();

    const retriedRequest = httpTesting.expectOne('/api/orders');
    retriedRequest.flush([]);

    expect(response?.status).toBe(200);
  });

  it('does not redirect while the login guard checks an unauthenticated session', () => {
    authService.refresh = vi.fn().mockReturnValue(throwError(() =>
      new HttpErrorResponse({ status: 401, statusText: 'Unauthorized' })));

    let responseError: HttpErrorResponse | undefined;

    http.get('/api/auth/me')
      .subscribe({ error: error => responseError = error });

    const request = httpTesting.expectOne('/api/auth/me');
    request.flush(null, { status: 401, statusText: 'Unauthorized' });

    expect(responseError?.status).toBe(401);
    expect(authService.clearSession).toHaveBeenCalledOnce();
    expect(router.navigate).not.toHaveBeenCalled();
  });

  it('clears the session and redirects when refresh fails for a protected request', () => {
    authService.refresh = vi.fn().mockReturnValue(throwError(() =>
      new HttpErrorResponse({ status: 401, statusText: 'Unauthorized' })));

    http.get('/api/orders').subscribe({ error: () => undefined });

    const request = httpTesting.expectOne('/api/orders');
    request.flush(null, { status: 401, statusText: 'Unauthorized' });

    expect(authService.clearSession).toHaveBeenCalledOnce();
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('does not clear the session when the retried request returns a non-auth error', () => {
    let responseError: HttpErrorResponse | undefined;

    http.get('/api/orders')
      .subscribe({ error: error => responseError = error });

    const initialRequest = httpTesting.expectOne('/api/orders');
    initialRequest.flush(null, { status: 401, statusText: 'Unauthorized' });

    const retriedRequest = httpTesting.expectOne('/api/orders');
    retriedRequest.flush(null, { status: 500, statusText: 'Server Error' });

    expect(responseError?.status).toBe(500);
    expect(authService.clearSession).not.toHaveBeenCalled();
    expect(router.navigate).not.toHaveBeenCalled();
  });
});
