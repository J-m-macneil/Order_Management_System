import { PaginationQuery } from './pagination-query.model';

export interface AuditLogQuery extends PaginationQuery {
  searchTerm?: string;
  entityType?: string;
  action?: string;
  entityId?: number | null;
  performedByUserId?: number | null;
  from?: string;
  to?: string;
}
