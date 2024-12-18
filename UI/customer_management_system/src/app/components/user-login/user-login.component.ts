import { Component, OnDestroy, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { UserLoginService } from '../../../../src/app/services/user-login.service';
import { NavbarService } from '../../../../src/app/services/navbar.service';
import { FooterService } from '../../../../src/app/services/footer.service';
import { UserLoginRequest } from '../../../../src/app/interfaces/user-login-request';
import { environment } from '../../../environments/environment';
import { SessionStorageService } from '../../../../src/app/services/session-storage.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-login',
  templateUrl: './user-login.component.html',
  styleUrls: ['./user-login.component.css'],
  imports: [CommonModule, ReactiveFormsModule],
})
export class UserLoginComponent implements OnInit, OnDestroy {
  loginForm: FormGroup;
  errorMessage: string | null = null;
  isEmailValid: boolean = true;

  constructor(
    private formBuilder: FormBuilder,
    private userLoginService: UserLoginService,
    private navbarService: NavbarService,
    private footerService: FooterService,
    private sessionStorageService: SessionStorageService
  ) {
    this.loginForm = this.formBuilder.group({
      username: [
        '',
        [Validators.required, Validators.pattern(environment.UserName)],
      ],
      password: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    // Clear session storage and prepare UI
    this.sessionStorageService.removeSessionStorage();
    this.navbarService.hideNavbar();
    this.footerService.hideFooter();

    // Subscribe to login error messages
    this.userLoginService.errorSubject.subscribe((message) => {
      this.errorMessage = message;
    });
  }

  get usernameControl() {
    return this.loginForm.get('username');
  }

  get passwordControl() {
    return this.loginForm.get('password');
  }

  validateEmail(): void {
    this.isEmailValid = this.usernameControl?.valid || false;
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      const loginRequest: UserLoginRequest = {
        merchantID: this.usernameControl?.value,
        merchantPassword: this.passwordControl?.value,
      };

      this.userLoginService.login(loginRequest).subscribe({
        next: (response) => {
          this.userLoginService.checkcredentials(response);
        },
        error: (error) => {
          this.handleLoginError(error.status);
        },
      });
    } else {
      this.errorMessage =
        'Please fill out all required fields with valid data.';
    }
  }

  private handleLoginError(statusCode: number): void {
    switch (statusCode) {
      case 403:
        this.errorMessage = 'Merchant credentials are incorrect!';
        break;
      case 404:
        this.errorMessage = 'Endpoint is down!';
        break;
      default:
        this.errorMessage = 'Server is down!';
    }
    this.userLoginService.errorSubject.next(this.errorMessage);
  }

  ngOnDestroy(): void {
    // Restore UI visibility
    this.navbarService.displayNavbar();
    this.footerService.displayFooter();
  }
}
