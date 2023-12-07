import { Injectable } from '@angular/core';
import {
  HttpClient,
  HttpHeaders,
  HttpParams,
  HttpParamsOptions,
} from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { UserLoginResponse } from 'src/app/interfaces/user-login-response';
import { UserLoginRequest } from 'src/app/interfaces/user-login-request';
import { GenericResponse } from 'src/app/interfaces/generic-response';
import { BehaviorSubject, of } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class UserLoginService {
  readonly APIURL =
    environment.CustomerManagementSystemAPI + '/Authentication/GetAccessToken';

  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*',
    }),
  };
  userSubject = new BehaviorSubject<any>(null);
  errorSubject = new BehaviorSubject<any>(null);
  errorMessage = this.errorSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {}

  userLoginResponse!: UserLoginResponse;
  accessToken!: string;

  login(userLoginRequest: UserLoginRequest): Observable<UserLoginResponse> {
    return this.http.post<UserLoginResponse>(
      this.APIURL,
      userLoginRequest,
      this.httpOptions
    );
  }

  checkcredentials(response: UserLoginResponse): void {
    if (
      response.responseCode == '200' &&
      response.responseMessage == 'Success!' &&
      response.accessToken != null
    ) {
      sessionStorage.setItem('accessToken', response.accessToken);
      this.router.navigateByUrl('customers');
    } else if (response.responseCode == '403') {
      this.errorSubject.next(response.responseMessage);
    } else if (response.responseCode == '404') {
      this.errorSubject.next(response.responseMessage);
    } else {
      this.errorSubject.next('Our servers are down!');
    }
  }
}
