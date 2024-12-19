import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { GetCustomerService } from '../../../../src/app/services/get-customer.service';
import {
  Address,
  Customer,
} from '../../../../src/app/interfaces/get-customer-list-response';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-edit-customer',
  templateUrl: './edit-customer.component.html',
  styleUrls: ['./edit-customer.component.css'],
  imports: [CommonModule, ReactiveFormsModule],
})
export class EditCustomerComponent implements OnInit {
  form!: FormGroup;
  genderDropdown: any = ['unknown', 'male', 'female'];
  loading = false;
  loadCompleted: boolean = false;
  submitted = false;
  customer = {} as Customer;
  customerAddress = {} as Address;

  get f() {
    return this.form.controls;
  }

  constructor(
    private getCustomerService: GetCustomerService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      msisdn: ['', Validators.required],
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
    this.loadCompleted = false;
    let paramID: string = this.activatedRoute.snapshot.queryParamMap.get('id')!;
    this.getCustomerService.getCustomer(paramID).subscribe({
      next: (response) => {
        this.customer = response;
        this.form.patchValue({
          firstName: this.customer.firstName,
          lastName: this.customer.lastName,
          email: this.customer.email,
          msisdn: this.customer.msisdn,
          gender: this.customer.gender,
          birthYear: this.customer.birthdate.split('-')[0],
          birthMonth: this.customer.birthdate.split('-')[1],
          birthDay: this.customer.birthdate.split('-')[2],
          country: this.customer.address.country,
          county: this.customer.address.county,
          town: this.customer.address.town,
          street: this.customer.address.street,
          number: this.customer.address.number,
          zip: this.customer.address.zip,
        });
        this.loadCompleted = true;
      },
      error: (error) => {
        if (error.error.responseCode == 403) {
          this.router.navigate(['']);
        } else if (error.error.responseCode == 404) {
        }
      },
    });
  }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.form.invalid) {
      return;
    }

    this.loading = true;

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
  }
}
