export interface AuditLogSummary {
  latestActivityText: string;
  latestActivityTime?: string | null;
  failedActionCount: number;
  systemActionCount: number;
}
