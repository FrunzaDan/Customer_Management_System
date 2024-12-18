import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, map, Observable, of } from 'rxjs';
import { VerifyTokenService } from '../../../src/app/services/verify-token.service';
import { LocalStorageService } from './local-storage.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuardService {
  constructor(
    private verifyTokenService: VerifyTokenService,
    private router: Router,
    private localStorageService: LocalStorageService
  ) {}

  canActivate(): Observable<boolean> {
    return this.verifyTokenService.isTokenValid().pipe(
      map((isTokenValid: boolean) => {
        return isTokenValid ? true : this.redirectToLogin();
      }),
      catchError((error) => this.handleError(error))
    );
  }

  private redirectToLogin(): boolean {
    this.logout();
    this.router.navigate(['login'], { queryParams: { sessionExpired: true } });
    return false;
  }

  private logout(): void {
    this.localStorageService.removeLocalAccessToken();
  }

  private handleError(error: any): Observable<boolean> {
    console.error('Error verifying token:', error);
    this.redirectToLogin();
    return of(false);
  }
}
