import { Injectable, Injector } from '@angular/core';
import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest
} from '@angular/common/http';
import { Router } from '@angular/router';
import {
  Observable,
  catchError,
  finalize,
  shareReplay,
  switchMap,
  throwError
} from 'rxjs';
import { AuthService } from '../auth/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private refreshRequest$: Observable<void> | null = null;

  constructor(
    private injector: Injector,
    private router: Router
  ) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const request = req.clone({ withCredentials: true });

    return next.handle(request).pipe(
      catchError(error => {
        if (!(error instanceof HttpErrorResponse) ||
            error.status !== 401 ||
            this.isSessionEndpoint(request.url)) {
          return throwError(() => error);
        }

        return this.refreshSession().pipe(
          switchMap(() => next.handle(request)),
          catchError(refreshError => {
            this.authService.clearSession();
            void this.router.navigate(['/login']);
            return throwError(() => refreshError);
          })
        );
      })
    );
  }

  private refreshSession(): Observable<void> {
    if (!this.refreshRequest$) {
      this.refreshRequest$ = this.authService.refresh().pipe(
        finalize(() => this.refreshRequest$ = null),
        shareReplay(1)
      );
    }

    return this.refreshRequest$;
  }

  private isSessionEndpoint(url: string): boolean {
    return ['/auth/login', '/auth/refresh', '/auth/logout']
      .some(path => url.endsWith(path));
  }

  private get authService(): AuthService {
    return this.injector.get(AuthService);
  }
}
