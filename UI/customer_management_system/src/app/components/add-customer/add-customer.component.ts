import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { first } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { AddCustomerService } from '../../../../src/app/services/add-customer.service';
import { SessionStorageService } from '../../../../src/app/services/session-storage.service';
import { Address, Customer } from '../../interfaces/customer-response';
import { environment } from '../../../environments/environment';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-add-customer',
  templateUrl: './add-customer.component.html',
  styleUrls: ['./add-customer.component.css'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
})
export class AddCustomerComponent implements OnInit {
  form!: FormGroup;
  loading: boolean = false;
  loadCompleted: boolean = false;
  submitted: boolean = false;
  errorMessage: string | null = null;
  customer = {} as Customer;
  customerAddress: Address = {} as Address;

  get f() {
    return this.form.controls;
  }

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private addCustomerService: AddCustomerService,
    private sessionStorageService: SessionStorageService,
  ) {}

  ngOnInit() {
    this.form = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: [
        '',
        [Validators.required, Validators.pattern(environment.EmailRegex)],
      ],
      msisdn: [
        '',
        [Validators.required, Validators.pattern(environment.PhoneRegex)],
      ],
      gender: ['', Validators.required],
      birthYear: ['', Validators.required],
      birthMonth: ['', Validators.required],
      birthDay: ['', Validators.required],
      country: ['', Validators.required],
      county: ['', Validators.required],
      town: ['', Validators.required],
      street: ['', Validators.required],
      number: ['', Validators.required],
      zip: ['', Validators.required],
    });
  }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.form.invalid) {
      return;
    }

    this.loading = true;
    this.errorMessage = null;

    this.customer.firstName = this.form.value.firstName;
    this.customer.lastName = this.form.value.lastName;
    this.customer.email = this.form.value.email;
    this.customer.msisdn = this.form.value.msisdn;
    this.customer.gender = this.form.value.gender;
    this.customer.birthdate =
      this.form.value.birthYear +
      '-' +
      this.form.value.birthMonth +
      '-' +
      this.form.value.birthDay;

    this.customerAddress.country = this.form.value.country;
    this.customerAddress.county = this.form.value.county;
    this.customerAddress.town = this.form.value.town;
    this.customerAddress.street = this.form.value.street;
    this.customerAddress.number = this.form.value.number;
    this.customerAddress.zip = this.form.value.zip;

    this.customer.address = this.customerAddress;

    this.addCustomerService
      .addCustomer(this.customer)
      .pipe(first())
      .subscribe({
        next: () => {
          this.router.navigate(['../customers'], { relativeTo: this.route });
        },
        error: (error: HttpErrorResponse) => {
          this.loading = false;

          // The register endpoint itself never returns 403 — a 401 here means the
          // session's JWT expired while filling out this form, since the auth guard
          // only re-checks the token on route navigation, not on every API call.
          if (error.status === 401) {
            this.sessionStorageService.removeSessionStorage();
            this.router.navigate(['login'], {
              queryParams: { sessionExpired: true },
            });
            return;
          }

          this.errorMessage = this.extractErrorMessage(error);
        },
      });
  }

  private extractErrorMessage(error: HttpErrorResponse): string {
    if (error.status === 0) {
      return 'Could not reach the server. It may be offline, or your browser does not trust its security certificate.';
    }
    return (
      error.error?.responseMessage ??
      error.error?.message ??
      `Failed to add customer (${error.status}). Please try again.`
    );
  }
}
