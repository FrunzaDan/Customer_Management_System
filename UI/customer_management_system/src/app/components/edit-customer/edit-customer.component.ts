import { CommonModule } from '@angular/common';
import {
  Component,
  effect,
  inject,
  Injector,
  OnInit,
  runInInjectionContext,
  ChangeDetectionStrategy,
  signal,
} from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Customer } from '../../interfaces/customer-response';
import { GetCustomerService } from '../../../../src/app/services/get-customer.service';
import { environment } from '../../../environments/environment';
import { EditCustomerService } from '../../services/edit-customer.service';

@Component({
  selector: 'app-edit-customer',
  templateUrl: './edit-customer.component.html',
  styleUrls: ['./edit-customer.component.css'],
  standalone: true,
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
})
export class EditCustomerComponent implements OnInit {
  form: FormGroup;
  paramId: string = '';
  submitted: boolean = false;

  readonly customer;
  readonly isLoading;
  readonly errorMessage;

  // Distinct from isLoading/errorMessage above, which reflect fetching the
  // customer being edited — this tracks the save (PATCH) request itself.
  readonly saving = signal(false);
  readonly saveError = signal<string | null>(null);

  private injector = inject(Injector);

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private getCustomerService: GetCustomerService,
    private editCustomerService: EditCustomerService,
  ) {
    this.form = this.createForm();

    this.customer = this.getCustomerService.selectedCustomerSignal;
    this.isLoading = this.getCustomerService.loadingSignal;
    this.errorMessage = this.getCustomerService.errorSignal;

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
              gender: customerData.gender?.toString() ?? '',
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

    this.saving.set(true);
    this.saveError.set(null);

    this.editCustomerService.editCustomer(updatedCustomer).subscribe({
      next: () => {
        this.router.navigate(['../customers'], { relativeTo: this.route });
      },
      error: (error: HttpErrorResponse) => {
        this.saving.set(false);
        this.saveError.set(this.extractErrorMessage(error));
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
      `Failed to save changes (${error.status}). Please try again.`
    );
  }
}
