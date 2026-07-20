import { ChangeDetectorRef, Component, Input, OnInit } from '@angular/core';
import { forkJoin } from 'rxjs';

import { AuthService } from '../../../core/auth/auth.service';
import { AuditLog } from '../../../core/models/audit-log.model';
import { AuditLogsService } from '../../../core/services/audit-logs.service';

@Component({
  selector: 'app-product-audit-panel',
  standalone: false,
  templateUrl: './product-audit-panel.component.html'
})
export class ProductAuditPanelComponent implements OnInit {
  @Input({ required: true }) productId!: number;

  auditLogs: AuditLog[] = [];
  auditUnavailable = false;
  isLoading = false;

  constructor(
    private auditLogsService: AuditLogsService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.reload();
  }

  reload(): void {
    if (!this.authService.hasRole('Admin', 'Demo')) {
      this.auditLogs = [];
      this.auditUnavailable = true;
      this.isLoading = false;
      return;
    }

    this.isLoading = true;

    forkJoin([
      this.auditLogsService.getAuditLogs({
        pageNumber: 1,
        pageSize: 10,
        entityType: 'Product',
        entityId: this.productId
      }),
      this.auditLogsService.getAuditLogs({
        pageNumber: 1,
        pageSize: 10,
        searchTerm: `product #${this.productId}`,
        entityType: 'SafetyDataSheet'
      })
    ]).subscribe({
      next: ([productResult, sdsResult]) => {
        this.auditLogs = [...productResult.items, ...sdsResult.items]
          .sort((a, b) => new Date(b.performedAt).getTime() - new Date(a.performedAt).getTime())
          .slice(0, 10);
        this.auditUnavailable = false;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: error => {
        console.error('Failed to load product audit history', error);
        this.auditLogs = [];
        this.auditUnavailable = true;
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  getEventLabel(log: AuditLog): string {
    if (log.changeSummary) {
      return log.entityType === 'SafetyDataSheet' ? 'SDS changed' : 'Product changed';
    }

    return `${log.entityType === 'SafetyDataSheet' ? 'SDS' : 'Product'} ${log.action.toLowerCase()}`;
  }

  getDescription(log: AuditLog): string {
    return log.changeSummary || log.notes || 'No audit note recorded';
  }
}
