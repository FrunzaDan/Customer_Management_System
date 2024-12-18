import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { UserLoginResponse } from '../../../src/app/interfaces/user-login-response';
import { UserLoginRequest } from '../../../src/app/interfaces/user-login-request';
import { BehaviorSubject, of } from 'rxjs';
import { environment } from '../../environments/environment';
import { SessionStorageService } from './session-storage.service';

@Injectable({
  providedIn: 'root',
})
export class UserLoginService {
  readonly APIURL =
    environment.CustomerManagementSystemAPI +
    '/api/Authentication/access-token';

  httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*',
    }),
  };
  userSubject = new BehaviorSubject<any>(null);
  errorSubject = new BehaviorSubject<any>(null);
  errorMessage = this.errorSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router,
    private sessionStorageService: SessionStorageService
  ) {}

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
      response.status == '200' &&
      response.responseMessage == 'Success!' &&
      response.accessToken != null
    ) {
      this.sessionStorageService.setSessionAccessToken(response.accessToken);
      this.router.navigateByUrl('customers');
    } else if (response.status == '403') {
      this.errorSubject.next(response.responseMessage);
    } else if (response.status == '404') {
      this.errorSubject.next(response.responseMessage);
    } else {
      this.errorSubject.next('Our servers are down!');
    }
  }
}
