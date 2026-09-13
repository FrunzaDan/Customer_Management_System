import { Component, OnInit, signal } from '@angular/core';
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
import { Address, Customer } from '../../interfaces/customer-response';
import { environment } from '../../../environments/environment';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-add-customer',
  templateUrl: './add-customer.component.html',
  styleUrls: ['./add-customer.component.css'],
  imports: [NgClass, ReactiveFormsModule, RouterLink],
})
export class AddCustomerComponent implements OnInit {
  form!: FormGroup;
  readonly loading = signal(false);
  readonly submitted = signal(false);
  readonly errorMessage = signal<string | null>(null);
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
      birthdate: ['', Validators.required],
      country: ['', Validators.required],
      county: ['', Validators.required],
      town: ['', Validators.required],
      street: ['', Validators.required],
      number: ['', Validators.required],
      zip: ['', Validators.required],
    });
  }

  onSubmit() {
    this.submitted.set(true);

    // stop here if form is invalid
    if (this.form.invalid) {
      return;
    }

    this.loading.set(true);
    this.errorMessage.set(null);

    this.customer.firstName = this.form.value.firstName;
    this.customer.lastName = this.form.value.lastName;
    this.customer.email = this.form.value.email;
    this.customer.msisdn = this.form.value.msisdn;
    this.customer.gender = this.form.value.gender;
    this.customer.birthdate = this.form.value.birthdate;

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
          this.loading.set(false);
          // A 401 here (session expired while filling out this form) is handled
          // globally by authErrorInterceptor, which redirects to login.
          this.errorMessage.set(this.extractErrorMessage(error));
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
