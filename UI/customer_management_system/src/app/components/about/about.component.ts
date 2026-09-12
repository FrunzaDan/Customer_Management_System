import { Component, ChangeDetectionStrategy, inject } from '@angular/core';
import { ApiLoggerService } from '../../services/api-logger.service';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-about',
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.css'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [],
})
export class AboutComponent {
  private readonly apiLoggerService = inject(ApiLoggerService);
  private readonly notificationService = inject(NotificationService);

  readonly apiLoggingEnabled = this.apiLoggerService.enabled;

  toggleApiLogging(): void {
    this.apiLoggerService.toggle();
    this.notificationService.show(
      `API call logging turned ${this.apiLoggingEnabled() ? 'on' : 'off'}.`,
    );
  }
}
