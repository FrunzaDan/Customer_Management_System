import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams, HttpParamsOptions } from '@angular/common/http';
import { Router } from '@angular/router';
import { map, catchError } from 'rxjs/operators';
import { UserLoginResponse } from 'src/app/interfaces/user-login-response';
import { UserLoginRequest } from 'src/app/interfaces/user-login-request';
import { GenericResponse } from 'src/app/interfaces/generic-response';
import { Subject, BehaviorSubject, of, Observable, throwError } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class VerifyTokenService {
    readonly APIURL = "https://localhost:7145/Authentication/VerifyToken";
    httpOptions = {
        headers: new HttpHeaders({
            'Content-Type': 'application/json',
            'Access-Control-Allow-Origin': '*'
        }),
    };

    constructor(private http: HttpClient, private router: Router) { }


    isTokenValid(): Observable<boolean> {

        const result = new Subject<boolean>();

        this.verifyTokenViaAPI().subscribe({
            next: response => {
                if (response.responseCode == '200') {
                    result.next(true);
                }
                else if (response.responseCode == '403') {
                    result.next(false);
                    result.complete();
                }
                else {
                    result.next(false);
                    result.complete();
                }
            },
            error: (error) => {
                if (error.error.responseCode == 403) {
                    console.log('Forbidden Access!')
                }

                result.next(false);
                result.complete();
            }
        });
        return result.asObservable();

    }

    verifyTokenViaAPI(): Observable<GenericResponse> {
        let accessTokenFromSession = sessionStorage.getItem('accessToken') || '{}';

        const headers = new HttpHeaders()
            .set('content-type', 'application/json')
            .set('Access-Control-Allow-Origin', '*');

        const params = new HttpParams()
            .set("accessToken", accessTokenFromSession);

        return this.http.get<GenericResponse>(this.APIURL, { headers: headers, params: params })
    }
}

