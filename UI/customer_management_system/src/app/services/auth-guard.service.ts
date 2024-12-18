import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { map, Observable } from 'rxjs';
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
      map((boolVal) => {
        if (boolVal == true) {
          return true;
        } else {
          this.logout();
          return false;
        }
      })
    );
  }

  logout() {
    this.localStorageService.removeLocalAccessToken();
    this.router.navigateByUrl('login');
  }
}
