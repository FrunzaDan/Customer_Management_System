import { Component, OnDestroy, ChangeDetectionStrategy } from '@angular/core';
import { NavbarService } from '../../services/navbar.service';
import { SessionStorageService } from '../../services/session-storage.service';
import { Subscription } from 'rxjs';

import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-navigation-bar',
  templateUrl: './navigation-bar.component.html',
  styleUrls: ['./navigation-bar.component.css'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [RouterModule],
})
export class NavigationBarComponent implements OnDestroy {
  showNavbar: boolean = true;
  subscription: Subscription;
  constructor(
    private navbarService: NavbarService,
    private sessionStorageService: SessionStorageService,
    private router: Router,
  ) {
    this.subscription = this.navbarService.showNavbar.subscribe((value) => {
      this.showNavbar = value;
    });
  }

  logout(): void {
    this.sessionStorageService.removeSessionStorage();
    this.router.navigate(['login']);
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
