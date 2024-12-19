import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { UserLoginResponse } from '../../../src/app/interfaces/user-login-response';
import { UserLoginRequest } from '../../../src/app/interfaces/user-login-request';
import { BehaviorSubject, of } from 'rxjs';
import { environment } from '../../environments/environment';
import { SessionStorageService } from './session-storage.service';
import { HttpHeaderService } from './http-header-service';

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
    private sessionStorageService: SessionStorageService,
    private httpHeaderService: HttpHeaderService
  ) {}

  userLoginResponse!: UserLoginResponse;
  accessToken!: string;

  login(userLoginRequest: UserLoginRequest): Observable<UserLoginResponse> {
    const headers = this.httpHeaderService.getHeadersWithTokenSet();
    return this.http.post<UserLoginResponse>(this.APIURL, userLoginRequest, {
      headers: headers,
    });
  }

  checkCredentials(response: UserLoginResponse): string {
    if (response.accessToken != null && response.status == 200) {
      this.sessionStorageService.setSessionAccessToken(response.accessToken);
      this.router.navigateByUrl('customers');
    }
    return response.responseMessage;
  }
}
