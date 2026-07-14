import { Component, OnInit, signal } from '@angular/core';
import { AuditLog } from '../../core/models/audit-log.model';
import { AuditLogsService } from '../../core/services/audit-logs.service';

@Component({
  selector: 'app-audit-logs',
  standalone: false,
  templateUrl: './audit-logs.component.html',
  styleUrls: ['./audit-logs.component.css']
})
export class AuditLogsComponent implements OnInit {
  readonly logs = signal<AuditLog[]>([]);
  readonly selectedLog = signal<AuditLog | null>(null);

  readonly pageNumber = signal(1);
  readonly pageSize = signal(25);
  readonly totalCount = signal(0);
  readonly totalPages = signal(0);
  readonly hasPreviousPage = signal(false);
  readonly hasNextPage = signal(false);

  entityTypes = ['Order', 'Customer', 'Product', 'ProcessingJob', 'Document', 'Notification'];
  actionTypes = [
    'Created',
    'Updated',
    'Deleted',
    'Generated',
    'Sent',
    'Completed',
    'Failed',
    'RetryQueued',
    'StatusChanged'
  ];

  searchTerm = '';
  entityType = '';
  action = '';
  entityId: number | null = null;
  performedByUserId: number | null = null;
  from = '';
  to = '';

  readonly isLoading = signal(false);
  readonly errorMessage = signal('');
  filtersVisible = false;

  constructor(private auditLogsService: AuditLogsService) { }

  ngOnInit(): void {
    this.loadAuditLogs();
  }

  loadAuditLogs(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.auditLogsService.getAuditLogs({
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      searchTerm: this.searchTerm.trim() || undefined,
      entityType: this.entityType || undefined,
      action: this.action.trim() || undefined,
      entityId: this.entityId,
      performedByUserId: this.performedByUserId,
      from: this.from || undefined,
      to: this.to || undefined
    }).subscribe({
      next: (result) => {
        this.logs.set(result.items);
        this.pageNumber.set(result.pageNumber);
        this.pageSize.set(result.pageSize);
        this.totalCount.set(result.totalCount);
        this.totalPages.set(result.totalPages);
        this.hasPreviousPage.set(result.hasPreviousPage);
        this.hasNextPage.set(result.hasNextPage);
        this.isLoading.set(false);
      },
      error: () => {
        this.logs.set([]);
        this.errorMessage.set('Failed to load audit logs.');
        this.isLoading.set(false);
      }
    });
  }

  applyFilters(): void {
    this.pageNumber.set(1);
    this.selectedLog.set(null);
    this.loadAuditLogs();
  }

  clearFilters(): void {
    this.entityType = '';
    this.action = '';
    this.entityId = null;
    this.performedByUserId = null;
    this.from = '';
    this.to = '';
    this.applyFilters();
  }

  toggleFilters(): void {
    this.filtersVisible = !this.filtersVisible;
  }

  selectLog(log: AuditLog): void {
    this.selectedLog.set(this.selectedLog()?.auditLogId === log.auditLogId
      ? null
      : log);
  }

  onPageChange(pageNumber: number): void {
    this.pageNumber.set(pageNumber);
    this.selectedLog.set(null);
    this.loadAuditLogs();
  }

  onPageSizeChange(value: number): void {
    this.pageSize.set(value);
    this.pageNumber.set(1);
    this.selectedLog.set(null);
    this.loadAuditLogs();
  }

  formatJson(value?: string | null): string {
    if (!value) {
      return 'None';
    }

    try {
      return JSON.stringify(JSON.parse(value), null, 2);
    } catch {
      return value;
    }
  }

  getEventLabel(log: AuditLog): string {
    if (log.changeSummary && log.action === 'Updated') {
      return `${this.formatEntityName(log.entityType)} changed`;
    }

    if (log.action.startsWith('StatusChanged:')) {
      const status = log.action.split(':')[1] || 'updated';
      const readableStatus = this.formatEntityName(status);

      if (log.entityType === 'Order' && status === 'Approved') {
        return 'Order approved';
      }

      if (log.entityType === 'Order' && (status === 'AwaitingDispatch' || status === 'Awaiting Dispatch')) {
        return 'Order awaiting dispatch';
      }

      return `${this.formatEntityName(log.entityType)} moved to ${readableStatus}`;
    }

    switch (log.action) {
      case 'Created':
        return `${this.formatEntityName(log.entityType)} created`;
      case 'Updated':
        return `${this.formatEntityName(log.entityType)} updated`;
      case 'Deleted':
        return `${this.formatEntityName(log.entityType)} deleted`;
      case 'Generated':
        return `${this.formatEntityName(log.entityType)} generated`;
      case 'Sent':
        return `${this.formatEntityName(log.entityType)} sent`;
      case 'Completed':
        return `${this.formatEntityName(log.entityType)} completed`;
      case 'Failed':
        return `${this.formatEntityName(log.entityType)} failed`;
      case 'RetryQueued':
        return `${this.formatEntityName(log.entityType)} retry queued`;
      default:
        return `${this.formatEntityName(log.entityType)} ${log.action}`;
    }
  }

  getActorLabel(log: AuditLog): string {
    if (log.performedByUserName && log.performedByUserId) {
      return `${log.performedByUserName} (#${log.performedByUserId})`;
    }

    return log.performedByUserName
      || (log.performedByUserId ? `User #${log.performedByUserId}` : 'System');
  }

  getAuditDescription(log: AuditLog): string {
    return log.changeSummary || log.notes || 'No notes recorded';
  }

  getActionClass(log: AuditLog): string {
    if (this.isFailedAction(log)) {
      return 'app-badge app-badge--danger';
    }

    if (!log.performedByUserId) {
      return 'app-badge app-badge--info';
    }

    if (log.action === 'Deleted') {
      return 'app-badge app-badge--warning';
    }

    return 'app-badge app-badge--neutral';
  }

  getEntityClass(entityType: string): string {
    switch (entityType) {
      case 'Order':
        return 'app-badge app-badge--info';
      case 'Customer':
        return 'app-badge app-badge--success';
      case 'Product':
        return 'app-badge app-badge--warning';
      case 'ProcessingJob':
        return 'app-badge app-badge--info';
      default:
        return 'app-badge app-badge--neutral';
    }
  }

  get activeFilterCount(): number {
    return [
      this.entityType,
      this.action,
      this.entityId,
      this.performedByUserId,
      this.from,
      this.to
    ].filter(Boolean).length;
  }

  private isFailedAction(log: AuditLog): boolean {
    return log.action.toLowerCase().includes('failed')
      || (log.notes?.toLowerCase().includes('failed') ?? false);
  }

  private formatEntityName(entityType: string): string {
    return entityType.replace(/([a-z])([A-Z])/g, '$1 $2');
  }
}
