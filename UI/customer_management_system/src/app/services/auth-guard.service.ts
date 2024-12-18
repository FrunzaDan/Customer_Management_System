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
      map((isValid: boolean) => {
        if (isValid) {
          return true;
        } else {
          this.handleInvalidToken();
          return false;
        }
      }),
      catchError((error) => {
        console.error('Error verifying token:', error);
        this.handleInvalidToken();
        return of(false);
      })
    );
  }

  private handleInvalidToken(): void {
    this.logout();
    this.router.navigate(['login'], { queryParams: { sessionExpired: true } });
  }

  logout(): void {
    this.localStorageService.removeLocalAccessToken();
  }
}
