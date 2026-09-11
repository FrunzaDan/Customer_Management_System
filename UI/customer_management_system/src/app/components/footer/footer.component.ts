import { Component, OnDestroy, ChangeDetectionStrategy } from '@angular/core';
import { FooterService } from '../../services/footer.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css'],
  changeDetection: ChangeDetectionStrategy.Eager,
  imports: [],
})
export class FooterComponent implements OnDestroy {
  showFooter: boolean = true;
  subscription: Subscription;
  constructor(private footerService: FooterService) {
    this.subscription = this.footerService.showFooter.subscribe((value) => {
      this.showFooter = value;
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
