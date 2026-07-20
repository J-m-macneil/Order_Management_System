import { of } from 'rxjs';

import { AuditLog } from '../../core/models/audit-log.model';
import { PagedResult } from '../../core/models/paged-result.model';
import { AuditLogsService } from '../../core/services/audit-logs.service';
import { AuditLogsComponent } from './audit-logs.component';

describe('AuditLogsComponent', () => {
  it('clears advanced filters without clearing search or selected state incorrectly', () => {
    const requests: Record<string, unknown>[] = [];
    const result: PagedResult<AuditLog> = {
      items: [],
      pageNumber: 1,
      pageSize: 25,
      totalCount: 0,
      totalPages: 0,
      hasPreviousPage: false,
      hasNextPage: false
    };
    const service = {
      getAuditLogs: (query: Record<string, unknown>) => {
        requests.push(query);
        return of(result);
      }
    } as unknown as AuditLogsService;
    const component = new AuditLogsComponent(service);

    component.searchTerm = 'Order approved';
    component.entityType = 'Order';
    component.action = 'Updated';
    component.entityId = 12;
    component.performedByUserId = 4;
    component.pageNumber.set(3);
    component.selectedLog.set({ auditLogId: 1 } as AuditLog);

    component.clearFilters();

    expect(component.searchTerm).toBe('Order approved');
    expect(component.entityType).toBe('');
    expect(component.action).toBe('');
    expect(component.entityId).toBeNull();
    expect(component.performedByUserId).toBeNull();
    expect(component.selectedLog()).toBeNull();
    expect(requests[0]['pageNumber']).toBe(1);
  });
});
