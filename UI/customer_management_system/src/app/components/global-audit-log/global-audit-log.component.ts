import {
  Component,
  OnInit,
  computed,
  inject,
  signal,
} from '@angular/core';
import { DatePipe } from '@angular/common';
import { Router } from '@angular/router';
import { GlobalAuditLogService } from '../../services/global-audit-log.service';
import { GlobalAuditLogEntry } from '../../interfaces/global-audit-log-entry';

@Component({
  selector: 'app-global-audit-log',
  templateUrl: './global-audit-log.component.html',
  styleUrls: ['./global-audit-log.component.css'],
  imports: [DatePipe],
})
export class GlobalAuditLogComponent implements OnInit {
  private readonly globalAuditLogService = inject(GlobalAuditLogService);
  private readonly router = inject(Router);

  readonly entries = this.globalAuditLogService.entriesSignal;
  readonly isLoading = this.globalAuditLogService.loadingSignal;
  readonly errorMessage = this.globalAuditLogService.errorSignal;
  readonly totalItems = this.globalAuditLogService.totalItemsSignal;

  readonly pageSize = 20;
  readonly currentPage = signal(1);

  readonly totalPages = computed(() =>
    Math.max(1, Math.ceil(this.totalItems() / this.pageSize)),
  );

  ngOnInit(): void {
    this.fetchAuditLog();
  }

  goToPage(page: number): void {
    const target = Math.min(Math.max(page, 1), this.totalPages());
    if (target === this.currentPage()) return;
    this.currentPage.set(target);
    this.fetchAuditLog();
  }

  // A deleted customer has no name to link to (see GlobalAuditLogEntry) —
  // only navigate when there's still a customer record behind the GUID.
  navigateToCustomer(entry: GlobalAuditLogEntry): void {
    if (!entry.customerFirstName && !entry.customerLastName) return;
    this.router.navigate(['/customerDetails'], {
      queryParams: { id: entry.customerGuid },
    });
  }

  customerLabel(entry: GlobalAuditLogEntry): string {
    if (!entry.customerFirstName && !entry.customerLastName) {
      return `(deleted customer ${entry.customerGuid})`;
    }
    return `${entry.customerFirstName ?? ''} ${entry.customerLastName ?? ''}`.trim();
  }

  trackByAuditId(_: number, entry: GlobalAuditLogEntry): number {
    return entry.auditId;
  }

  private fetchAuditLog(): void {
    this.globalAuditLogService.loadAllAuditLog({
      pageNumber: this.currentPage(),
      pageSize: this.pageSize,
    });
  }
}
