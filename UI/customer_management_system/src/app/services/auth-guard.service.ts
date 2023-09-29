import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { map, Observable } from 'rxjs';
import { VerifyTokenService } from 'src/app/services/verify-token.service';

@Injectable({
    providedIn: 'root'
})

export class AuthGuardService {
    constructor(private verifyTokenService: VerifyTokenService, private router: Router) { }

    canActivate(): Observable<boolean> {
        return this.verifyTokenService.isTokenValid().pipe(
            map(boolVal => {
                if (boolVal == true) {
                    return true;
                }
                else {
                    this.logout();
                    return false;
                }
            })
        );
    }

    logout() {
        localStorage.removeItem('accessToken');
        this.router.navigateByUrl('login');
    }
}
