import { Component } from '@angular/core';
import { GetCustomerService } from 'src/app/services/get-customer.service';
import {
  Address,
  Customer,
} from 'src/app/interfaces/get-customer-list-response';
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
  customerGender: string | undefined;
  loadCompleted: boolean = false;
  customer = {} as Customer;

  constructor(
    private getCustomerService: GetCustomerService,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loadCompleted = false;
    let paramID: string = this.activatedRoute.snapshot.queryParamMap.get('id')!;
    this.getCustomerService.getCustomer(paramID).subscribe({
      next: (response) => {
        this.customer = response;
        this.customerGender = this.genderMap.get(this.customer.gender);
        this.loadCompleted = true;
      },
      error: (error) => {
        let errorStatusCode = error.status;
        if (errorStatusCode == 403) {
          this.router.navigate(['']);
        } else if (errorStatusCode == 404) {
          this.loadCompleted = true;
        }
      },
      complete: () => {},
    });
  }
}
