import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CustomerListComponent } from '../customer-list/customer-list.component';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  imports: [CommonModule, CustomerListComponent],
})
export class HomeComponent {
  constructor(private router: Router) {}

  addCustomer(): void {
    this.router.navigate(['/addCustomer']);
  }
}
