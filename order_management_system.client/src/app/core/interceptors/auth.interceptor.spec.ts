import {
  HTTP_INTERCEPTORS,
  HttpClient,
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
import { of } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { AuthInterceptor } from './auth.interceptor';

describe('AuthInterceptor', () => {
  let http: HttpClient;
  let httpTesting: HttpTestingController;
  let authService: Pick<AuthService, 'refresh' | 'clearSession'>;

  beforeEach(() => {
    authService = {
      refresh: vi.fn().mockReturnValue(of(void 0)),
      clearSession: vi.fn()
    };

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
        { provide: Router, useValue: { navigate: vi.fn() } }
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
});
