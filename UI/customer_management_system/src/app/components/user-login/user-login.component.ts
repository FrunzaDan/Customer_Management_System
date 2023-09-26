import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { UserLoginService } from 'src/app/services/user-login.service';
import { NavbarService } from 'src/app/services/navbar.service';
import { FooterService } from 'src/app/services/footer.service';
import { UserLoginRequest } from 'src/app/interfaces/user-login-request';

@Component({
  selector: 'app-user-login',
  templateUrl: './user-login.component.html',
  styleUrls: ['./user-login.component.css']
})
export class UserLoginComponent implements OnInit, OnDestroy {
  userLoginRequest = {} as UserLoginRequest;
  username: string = '';
  password: string = '';
  isEmailValid: boolean = true;
  error: null = null;

  constructor(private builder: FormBuilder, private userLoginSerivce: UserLoginService,
    private navbarService: NavbarService, private footerService: FooterService) {
    sessionStorage.clear();
  }

  ngOnInit(): void {
    sessionStorage.clear();
    this.userLoginSerivce
      .errorSubject
      .subscribe(errorMessage => {
        this.error = errorMessage;
        console.log(this.error);
      });
    this.navbarService.hideNavbar();
    this.footerService.hideFooter();
  }

  validateEmail(): void {
    const pattern = RegExp(/^[a-zA-z0-9\ ]*$/);
    if (pattern.test(this.username)) {
      this.isEmailValid = true;
    } else {
      this.isEmailValid = false;
    }
  }

  onKey(event: any, type: string) {
    if (type === 'username') {
      this.username = event.target.value;
      this.validateEmail();
    }
    else if (type === 'password') {
      this.password = event.target.value;
    }
  }

  onSubmit() {
    if (this.isEmailValid) {
      this.userLoginRequest.merchantID = this.username;
      this.userLoginRequest.merchantPassword = this.password;
      this.userLoginSerivce
        .login(this.userLoginRequest).subscribe({
          next: response => {
            this.userLoginSerivce.checkcredentials(response);
          },
          error: () => this.userLoginSerivce.errorSubject.next('Our servers are down!')
        });
    }
  }

  loginform = this.builder.group({
    id: this.builder.control('', Validators.required),
    password: this.builder.control('', Validators.required)
  });

  ngOnDestroy(): void {
    this.navbarService.displayNavbar();
    this.footerService.displayFooter();
  }

}
