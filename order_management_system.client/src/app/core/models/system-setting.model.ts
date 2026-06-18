export interface SystemSetting {
  systemSettingId: number;
  settingKey: string;
  settingValue: string;
  dataType: 'integer' | 'decimal' | 'boolean' | 'string';
  description?: string | null;
  createdAt: string;
  updatedAt?: string | null;
}

export interface UpdateSystemSettingRequest {
  settingValue: string;
}
