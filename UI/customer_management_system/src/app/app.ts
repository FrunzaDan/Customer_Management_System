import { Component, ChangeDetectionStrategy } from '@angular/core';
import { FooterComponent } from './components/footer/footer.component';
import { NavigationBarComponent } from './components/navigation-bar/navigation-bar.component';
import { NotificationComponent } from './components/notification/notification.component';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrls: ['./app.css'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [
    FooterComponent,
    NavigationBarComponent,
    NotificationComponent,
    RouterOutlet,
  ],
})
export class App {
  title = 'customer_management_system';
}
