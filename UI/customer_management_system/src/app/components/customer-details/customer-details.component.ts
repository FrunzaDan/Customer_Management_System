import { Component, Signal, computed } from '@angular/core';
import { GetCustomerService } from '../../../../src/app/services/get-customer.service';
import { Customer } from '../../../../src/app/interfaces/get-customer-list-response';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-customer-details',
  templateUrl: './customer-details.component.html',
  styleUrls: ['./customer-details.component.css'],
})
export class CustomerDetailsComponent {
  genderMap = new Map<Customer['gender'], string>([
    [0, 'not declared'],
    [1, 'male'],
    [2, 'female'],
  ]);

  customer: Signal<Customer | null>;
  isLoading: Signal<boolean>;
  errorMessage: Signal<string | null>;
  customerGender: Signal<string | undefined>;

  constructor(
    private getCustomerService: GetCustomerService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
  ) {
    // Get signals from the service
    this.customer = this.getCustomerService.getCustomerSignal();
    console.log(this.customer());
    this.isLoading = this.getCustomerService.isLoading();
    this.errorMessage = this.getCustomerService.getErrorMessage();

    // Compute gender based on the customer data
    this.customerGender = computed(() => {
      const c = this.customer();
      return c && c.gender !== undefined
        ? this.genderMap.get(c.gender)
        : undefined;
    });
  }

  ngOnInit(): void {
    const paramID: string | null =
      this.activatedRoute.snapshot.queryParamMap.get('id');
    if (paramID) {
      this.getCustomerService.getCustomer(paramID);
    } else {
      this.router.navigate(['']); // Redirect if no ID is found
    }
  }
}
