import { CommonModule } from '@angular/common';
import {
  Component,
  effect,
  inject,
  Injector,
  OnInit,
  runInInjectionContext,
} from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Customer } from '../../../../src/app/interfaces/get-customer-list-response';
import { GetCustomerService } from '../../../../src/app/services/get-customer.service';
import { environment } from '../../../environments/environment';
import { EditCustomerService } from '../../services/edit-customer.service';

@Component({
  selector: 'app-edit-customer',
  templateUrl: './edit-customer.component.html',
  styleUrls: ['./edit-customer.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
})
export class EditCustomerComponent implements OnInit {
  form: FormGroup;
  genderDropdown: string[] = ['unknown', 'male', 'female'];
  paramId: string = '';
  submitted: boolean = false;

  readonly customer;
  readonly isLoading;
  readonly errorMessage;
  private injector = inject(Injector);

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private getCustomerService: GetCustomerService,
    private editCustomerService: EditCustomerService,
  ) {
    this.form = this.createForm();

    this.customer = this.getCustomerService.selectedCustomer;
    this.isLoading = this.getCustomerService.loading;
    this.errorMessage = this.getCustomerService.error;

    // Create effect in constructor using injector
    runInInjectionContext(this.injector, () => {
      effect(() => {
        const customerData = this.customer();
        if (customerData) {
          const birthParts = customerData.birthdate.split('-');

          this.form.patchValue(
            {
              firstName: customerData.firstName,
              lastName: customerData.lastName,
              email: customerData.email,
              msisdn: customerData.msisdn,
              gender: customerData.gender,
              birthYear: birthParts[0],
              birthMonth: birthParts[1],
              birthDay: birthParts[2],
              country: customerData.address.country,
              county: customerData.address.county,
              town: customerData.address.town,
              street: customerData.address.street,
              number: customerData.address.number,
              zip: customerData.address.zip,
            },
            { emitEvent: false },
          );
        }
      });
    });
  }

  private createForm(): FormGroup {
    return this.fb.group({
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

  ngOnInit(): void {
    this.paramId = this.route.snapshot.queryParamMap.get('id') ?? '';
    if (this.paramId) {
      this.getCustomerService.getCustomer(this.paramId);
    }
  }

  get f() {
    return this.form.controls;
  }

  onSubmit() {
    this.submitted = true;

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
