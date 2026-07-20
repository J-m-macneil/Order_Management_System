import { apiBaseUrl } from '../config/api-url';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { SystemSetting, UpdateSystemSettingRequest } from '../models/system-setting.model';

@Injectable({
  providedIn: 'root'
})
export class SystemSettingsService {
  private readonly baseUrl = `${apiBaseUrl}/system-settings`;

  constructor(private http: HttpClient) { }

  getSettings(): Observable<SystemSetting[]> {
    return this.http.get<SystemSetting[]>(this.baseUrl);
  }

  update(systemSettingId: number, request: UpdateSystemSettingRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${systemSettingId}`, request);
  }
}
