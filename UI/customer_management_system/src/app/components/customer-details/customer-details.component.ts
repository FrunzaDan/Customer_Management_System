import { Component, Signal, computed, ChangeDetectionStrategy } from '@angular/core';
import { GetCustomerService } from '../../../../src/app/services/get-customer.service';
import { Customer } from '../../interfaces/customer-response';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-customer-details',
  templateUrl: './customer-details.component.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrls: ['./customer-details.component.css'],
})
export class CustomerDetailsComponent {
  genderMap = new Map<Customer['gender'], string>([
    [0, 'not declared'],
    [1, 'male'],
    [2, 'female'],
  ]);

  readonly customer;
  readonly isLoading;
  readonly errorMessage;
  customerGender: Signal<string | undefined>;

  constructor(
    private getCustomerService: GetCustomerService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
  ) {
    this.customer = this.getCustomerService.selectedCustomerSignal;
    this.isLoading = this.getCustomerService.loadingSignal;
    this.errorMessage = this.getCustomerService.errorSignal;

    this.customerGender = computed(() => {
      const c = this.customer();
      return c && c.gender !== undefined
        ? this.genderMap.get(c.gender)
        : undefined;
    });
  }

  ngOnInit(): void {
    this.activatedRoute.queryParamMap.subscribe((params) => {
      const paramID = params.get('id');
      if (paramID) {
        this.getCustomerService.getCustomer(paramID);
      } else {
        this.router.navigate(['']);
      }
    });
  }
}
