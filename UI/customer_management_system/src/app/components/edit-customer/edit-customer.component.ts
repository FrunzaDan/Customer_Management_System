import { Component, OnInit, Signal, computed, effect } from '@angular/core';
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
import { environment } from '../../../environments/environment';
import { CommonModule } from '@angular/common';
import { EditCustomerService } from '../../services/edit-customer.service';

@Component({
  selector: 'app-edit-customer',
  templateUrl: './edit-customer.component.html',
  styleUrls: ['./edit-customer.component.css'],
  imports: [CommonModule, ReactiveFormsModule],
})
export class EditCustomerComponent implements OnInit {
  form!: FormGroup;
  genderDropdown: any = ['unknown', 'male', 'female'];
  paramId: string = '';
  submitted: boolean = false; // ✅ Add this property

  // Using signals
  customer: Signal<Customer | null>;
  isLoading: Signal<boolean>;
  errorMessage: Signal<string | null>;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private getCustomerService: GetCustomerService,
    private editCustomerService: EditCustomerService,
  ) {
    this.customer = this.getCustomerService.getCustomerSignal();
    this.isLoading = this.getCustomerService.isLoading();
    this.errorMessage = this.getCustomerService.getErrorMessage();
  }

  ngOnInit(): void {
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

    this.paramId = this.route.snapshot.queryParamMap.get('id')!;
    this.getCustomerService.getCustomer(this.paramId);

    effect(() => {
      const customerData = this.customer();
      if (customerData) {
        this.form.patchValue({
          firstName: customerData.firstName,
          lastName: customerData.lastName,
          email: customerData.email,
          msisdn: customerData.msisdn,
          gender: customerData.gender,
          birthYear: customerData.birthdate.split('-')[0],
          birthMonth: customerData.birthdate.split('-')[1],
          birthDay: customerData.birthdate.split('-')[2],
          country: customerData.address.country,
          county: customerData.address.county,
          town: customerData.address.town,
          street: customerData.address.street,
          number: customerData.address.number,
          zip: customerData.address.zip,
        });
      }
    });
  }

  // ✅ Getter for form controls
  get f() {
    return this.form.controls;
  }

  onSubmit() {
    this.submitted = true; // ✅ Track form submission

    if (this.form.invalid || !this.customer()) {
      return;
    }

    const updatedCustomer: Customer = {
      ...this.customer()!,
      firstName: this.form.value.firstName,
      lastName: this.form.value.lastName,
      email: this.form.value.email,
      msisdn: this.form.value.msisdn,
      gender: this.form.value.gender,
      birthdate: `${this.form.value.birthYear}-${this.form.value.birthMonth}-${this.form.value.birthDay}`,
      address: {
        country: this.form.value.country,
        county: this.form.value.county,
        town: this.form.value.town,
        street: this.form.value.street,
        number: this.form.value.number,
        zip: this.form.value.zip,
      },
    };

    this.editCustomerService.editCustomer(updatedCustomer);
    this.router.navigate(['../customers'], { relativeTo: this.route });
  }
}
