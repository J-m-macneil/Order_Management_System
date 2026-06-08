import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuditLog } from '../models/audit-log.model';
import { AuditLogQuery } from '../models/audit-log-query.model';
import { AuditLogSummary } from '../models/audit-log-summary.model';
import { PagedResult } from '../models/paged-result.model';

@Injectable({
  providedIn: 'root'
})
export class AuditLogsService {
  private readonly baseUrl = 'https://localhost:7233/api/audit-logs';

  constructor(private http: HttpClient) { }

  getAuditLogs(query: AuditLogQuery): Observable<PagedResult<AuditLog>> {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize);

    if (query.searchTerm) {
      params = params.set('searchTerm', query.searchTerm);
    }

    if (query.entityType) {
      params = params.set('entityType', query.entityType);
    }

    if (query.action) {
      params = params.set('action', query.action);
    }

    if (query.entityId) {
      params = params.set('entityId', query.entityId);
    }

    if (query.performedByUserId) {
      params = params.set('performedByUserId', query.performedByUserId);
    }

    if (query.from) {
      params = params.set('from', query.from);
    }

    if (query.to) {
      params = params.set('to', query.to);
    }

    return this.http.get<PagedResult<AuditLog>>(this.baseUrl, { params });
  }

  getSummary(query: Omit<AuditLogQuery, 'pageNumber' | 'pageSize'>): Observable<AuditLogSummary> {
    let params = new HttpParams();

    if (query.searchTerm) {
      params = params.set('searchTerm', query.searchTerm);
    }

    if (query.entityType) {
      params = params.set('entityType', query.entityType);
    }

    if (query.action) {
      params = params.set('action', query.action);
    }

    if (query.entityId) {
      params = params.set('entityId', query.entityId);
    }

    if (query.performedByUserId) {
      params = params.set('performedByUserId', query.performedByUserId);
    }

    if (query.from) {
      params = params.set('from', query.from);
    }

    if (query.to) {
      params = params.set('to', query.to);
    }

    return this.http.get<AuditLogSummary>(`${this.baseUrl}/summary`, { params });
  }
}
