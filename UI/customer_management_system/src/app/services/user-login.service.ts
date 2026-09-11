import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import {
  LoginData,
  LoginDataResponse,
} from '../../../src/app/interfaces/user-login-response';
import { UserLoginRequest } from '../../../src/app/interfaces/user-login-request';
import { BehaviorSubject, of } from 'rxjs';
import { environment } from '../../environments/environment';
import { SessionStorageService } from './session-storage.service';
import { HttpHeaderService } from './http-header-service';
import { GenericResponse } from '../interfaces/generic-response';
import { NotificationService } from './notification.service';

@Injectable({
  providedIn: 'root',
})
export class UserLoginService {
  readonly APIURL =
    environment.CustomerManagementSystemAPI +
    '/api/Authentication/access-token';

  userSubject = new BehaviorSubject<any>(null);
  errorSubject = new BehaviorSubject<any>(null);
  errorMessage = this.errorSubject.asObservable();

  constructor(
    private http: HttpClient,
    private router: Router,
    private sessionStorageService: SessionStorageService,
    private httpHeaderService: HttpHeaderService,
    private notificationService: NotificationService,
  ) {}

  userLoginResponse!: LoginDataResponse;
  accessToken!: string;

  login(userLoginRequest: UserLoginRequest): Observable<LoginDataResponse> {
    const headers = this.httpHeaderService.getHeadersWithTokenSet();
    return this.http.post<GenericResponse<LoginData>>(
      this.APIURL,
      userLoginRequest,
      {
        headers: headers,
      },
    );
  }

  checkCredentials(response: LoginDataResponse): string {
    if (response.data?.accessToken != null && response.status == 200) {
      this.sessionStorageService.setSessionAccessToken(
        response.data.accessToken,
      );
      this.notificationService.show('Login successful.');
      this.router.navigateByUrl('customers');
    }
    return response.responseMessage;
  }
}
