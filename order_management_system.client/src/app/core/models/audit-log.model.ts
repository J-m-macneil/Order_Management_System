export interface AuditLog {
  auditLogId: number;
  entityType: string;
  entityId: number;
  action: string;
  performedByUserId?: number | null;
  performedByUserName?: string | null;
  performedAt: string;
  oldValuesJson?: string | null;
  newValuesJson?: string | null;
  notes?: string | null;
  changeSummary?: string | null;
}
