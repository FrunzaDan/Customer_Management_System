import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { GetCustomerService } from 'src/app/services/get-customer.service';
import { Address, Customer } from 'src/app/interfaces/get-customer-list-response';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-edit-customer',
  templateUrl: './edit-customer.component.html',
  styleUrls: ['./edit-customer.component.css']
})
export class EditCustomerComponent implements OnInit {
  form!: FormGroup;
  genderDropdown: any = ['unknown', 'male', 'female'];
  loading = false;
  loadCompleted: boolean = false;
  submitted = false;
  customer = {} as Customer;
  customerAddress = {} as Address;

  get f() { return this.form.controls; }

  constructor(private getCustomerService: GetCustomerService, private router: Router, private activatedRoute: ActivatedRoute) {
  }

  ngOnInit(): void {

    this.loadCompleted = false;
    let paramID: string = this.activatedRoute.snapshot.queryParamMap.get('id')!;
    this.getCustomerService.getCustomer(paramID).subscribe({
      next: response => {
        this.customer = response;
        this.loadCompleted = true;
      },
      error: error => {
        if (error.error.responseCode == 403) {
          this.router.navigate(['']);
        }
      }
    })
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
  }
}
