import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { UserLoginResponse } from 'src/app/interfaces/user-login-response';
import { UserLoginRequest } from 'src/app/interfaces/user-login-request';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserLoginService {
  readonly APIURL = "https://localhost:7145/Authentication/MerchantLogin";
  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*'
    }),
  };
  userSubject = new BehaviorSubject<any>(null);
  errorSubject = new BehaviorSubject<any>(null);
  errorMessage = this.errorSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) { }

  userLoginResponse!: UserLoginResponse;
  accessToken!: string;

  login(userLoginRequest: UserLoginRequest): Observable<UserLoginResponse> {
    return this.http.post<UserLoginResponse>(
      this.APIURL,
      userLoginRequest,
      this.httpOptions
    );
  };

  checkcredentials(response: UserLoginResponse) {
    if (response.responseCode == '200' && response.responseMessage == 'You Have Access Rights!') {
      sessionStorage.setItem('accessToken', response.accessToken)
      this.router.navigateByUrl('customers');
    }
    else if (response.responseCode == '403') {
      this.errorSubject.next(response.responseMessage);
    }
    else {
      this.errorSubject.next('Our servers are down!');
    }
  };
}

