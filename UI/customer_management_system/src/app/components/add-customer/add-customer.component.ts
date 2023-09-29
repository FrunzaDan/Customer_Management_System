import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';
import { AddCustomerService } from 'src/app/services/add-customer.service';
import { Customer, Address } from 'src/app/interfaces/get-customer-list-response';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-add-customer',
  templateUrl: './add-customer.component.html',
  styleUrls: ['./add-customer.component.css']
})
export class AddCustomerComponent implements OnInit {
  form!: FormGroup;
  genderDropdown: any = ['unknown', 'male', 'female'];
  loading = false;
  submitted = false;
  customer = {} as Customer;
  customerAddress = {} as Address;

  get f() { return this.form.controls; }

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private addCustomerService: AddCustomerService
  ) { }

  ngOnInit() {
    this.form = this.formBuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.pattern(environment.EmailRegex)]],
      msisdn: ['', [Validators.required, Validators.pattern(environment.PhoneRegex)]],
      gender: ['', Validators.required],
      birthYear: ['', Validators.required],
      birthMonth: ['', Validators.required],
      birthDay: ['', Validators.required],
      country: ['', Validators.required],
      county: ['', Validators.required],
      town: ['', Validators.required],
      street: ['', Validators.required],
      number: ['', Validators.required],
      zip: ['', Validators.required]
    });
  }

  onSubmit() {
    this.submitted = true;

    console.log("submitted");
    console.log("submitted");

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
    this.customer.birthdate = this.form.value.birthYear + "-" + this.form.value.birthMonth + "-" + this.form.value.birthDay;

    this.customerAddress.country = this.form.value.country;
    this.customerAddress.county = this.form.value.county;
    this.customerAddress.town = this.form.value.town;
    this.customerAddress.street = this.form.value.street;
    this.customerAddress.number = this.form.value.number;
    this.customerAddress.zip = this.form.value.zip;

    this.customer.address = this.customerAddress;

    this.addCustomerService.addCustomer(this.customer)
      .pipe(first())
      .subscribe({
        next: () => {
          this.router.navigate(['../customers'], { relativeTo: this.route });
        },
        error: error => {
          if (error.error.responseCode == 403) {
            this.router.navigate(['']);
          }
          this.loading = false;
        }
      });
  }

}
