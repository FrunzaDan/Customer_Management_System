import { Component, OnDestroy } from '@angular/core';
import { NavbarService } from '../../services/navbar.service';
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-navigation-bar',
  templateUrl: './navigation-bar.component.html',
  styleUrls: ['./navigation-bar.component.css'],
  imports: [CommonModule, RouterModule],
})
export class NavigationBarComponent implements OnDestroy {
  showNavbar: boolean = true;
  subscription: Subscription;
  constructor(private navbarService: NavbarService) {
    this.subscription = this.navbarService.showNavbar.subscribe((value) => {
      this.showNavbar = value;
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
