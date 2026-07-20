import { ChangeDetectorRef, Component, OnInit } from '@angular/core';

import { SystemSetting } from '../../../core/models/system-setting.model';
import { AuthService } from '../../../core/auth/auth.service';
import { SystemSettingsService } from '../../../core/services/system-settings.service';
import { ToastService } from '../../../core/services/toast.service';
import { ApiErrorResponse, getApiErrorMessage } from '../../../core/utils/api-error-message';

@Component({
  selector: 'app-system-settings',
  standalone: false,
  templateUrl: './system-settings.component.html'
})
export class SystemSettingsComponent implements OnInit {
  readonly settingGroups = ['Orders', 'Background Processing', 'Dashboard', 'Compliance'];
  settingsByGroup: Record<string, SystemSetting[]> = {};
  settingValues: Record<number, string> = {};
  savingSettingIds = new Set<number>();

  isLoading = false;
  errorMessage = '';
  readonly isDemoUser: boolean;

  constructor(
    private systemSettingsService: SystemSettingsService,
    private toastService: ToastService,
    private cdr: ChangeDetectorRef,
    authService: AuthService
  ) {
    this.isDemoUser = authService.isDemoUser();
  }

  ngOnInit(): void {
    this.loadSystemSettings();
  }

  loadSystemSettings(): void {
    this.isLoading = true;

    this.systemSettingsService.getSettings().subscribe({
      next: settings => {
        this.settingsByGroup = this.settingGroups.reduce<Record<string, SystemSetting[]>>((groups, group) => {
          groups[group] = settings.filter(setting => this.getSettingGroup(setting.settingKey) === group);
          return groups;
        }, {});
        this.settingValues = settings.reduce<Record<number, string>>((values, setting) => {
          values[setting.systemSettingId] = setting.settingValue;
          return values;
        }, {});
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: err => {
        console.error('Failed to load system settings', err);
        this.errorMessage = 'Failed to load system settings.';
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  saveSetting(setting: SystemSetting): void {
    const value = this.settingValues[setting.systemSettingId];
    const validationError = this.validateSettingValue(setting, value);

    if (validationError) {
      this.toastService.error('Invalid setting value', validationError);
      return;
    }

    this.errorMessage = '';
    this.savingSettingIds.add(setting.systemSettingId);

    this.systemSettingsService.update(setting.systemSettingId, { settingValue: value.trim() }).subscribe({
      next: () => {
        this.savingSettingIds.delete(setting.systemSettingId);
        this.toastService.success(
          'Setting updated',
          `${this.formatSettingName(setting.settingKey)} was saved.`
        );
        this.loadSystemSettings();
      },
      error: (err: ApiErrorResponse) => {
        console.error('Failed to update system setting', err);
        this.savingSettingIds.delete(setting.systemSettingId);
        this.toastService.error('Setting update failed', getApiErrorMessage(err, 'Failed to update system setting.'));
        this.cdr.markForCheck();
      }
    });
  }

  private getSettingGroup(settingKey: string): string {
    if (settingKey.includes('BackgroundJob')) {
      return 'Background Processing';
    }

    if (settingKey.includes('Dashboard')) {
      return 'Dashboard';
    }

    if (settingKey.includes('Sds') || settingKey.includes('Hazardous')) {
      return 'Compliance';
    }

    return 'Orders';
  }

  formatSettingName(settingKey: string): string {
    return settingKey.replace(/([a-z])([A-Z])/g, '$1 $2');
  }

  isSettingDirty(setting: SystemSetting): boolean {
    return this.settingValues[setting.systemSettingId] !== setting.settingValue;
  }

  isSavingSetting(setting: SystemSetting): boolean {
    return this.savingSettingIds.has(setting.systemSettingId);
  }

  isBooleanSetting(setting: SystemSetting): boolean {
    return setting.dataType.toLowerCase() === 'boolean';
  }

  setBooleanSetting(setting: SystemSetting, checked: boolean): void {
    this.settingValues[setting.systemSettingId] = checked ? 'true' : 'false';
  }

  resetSetting(setting: SystemSetting): void {
    this.settingValues[setting.systemSettingId] = setting.settingValue;
  }

  private validateSettingValue(setting: SystemSetting, value: string | undefined): string | null {
    if (!value || !value.trim()) {
      return `${this.formatSettingName(setting.settingKey)} requires a value.`;
    }

    const trimmed = value.trim();
    const dataType = setting.dataType.toLowerCase();

    if (dataType === 'integer' && !/^-?\d+$/.test(trimmed)) {
      return `${this.formatSettingName(setting.settingKey)} must be a whole number.`;
    }

    if (dataType === 'decimal' && Number.isNaN(Number(trimmed))) {
      return `${this.formatSettingName(setting.settingKey)} must be a decimal number.`;
    }

    if (dataType === 'boolean' && trimmed !== 'true' && trimmed !== 'false') {
      return `${this.formatSettingName(setting.settingKey)} must be true or false.`;
    }

    return null;
  }
}
