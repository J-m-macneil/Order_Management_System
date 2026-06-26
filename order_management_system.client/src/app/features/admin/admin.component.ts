import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { SystemSetting } from '../../core/models/system-setting.model';
import { Department, Role, User, UserSaveRequest } from '../../core/models/user-management.model';
import { SystemSettingsService } from '../../core/services/system-settings.service';
import { UsersService } from '../../core/services/users.service';

@Component({
  selector: 'app-admin',
  standalone: false,
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  activeSection: 'users' | 'settings' = 'users';

  users: User[] = [];
  roles: Role[] = [];
  departments: Department[] = [];
  systemSettings: SystemSetting[] = [];
  settingValues: Record<number, string> = {};
  savingSettingIds = new Set<number>();

  userForm: FormGroup;
  selectedUser: User | null = null;
  showUserForm = false;

  pageNumber = 1;
  pageSize = 25;
  totalCount = 0;
  totalPages = 0;
  hasPreviousPage = false;
  hasNextPage = false;
  pageSizeOptions = [25, 50, 100];

  searchTerm = '';
  roleFilter: number | null = null;
  statusFilter = '';
  filtersVisible = false;

  isLoading = false;
  isLoadingSettings = false;
  isSaving = false;
  errorMessage = '';
  settingsMessage = '';

  constructor(
    private usersService: UsersService,
    private systemSettingsService: SystemSettingsService,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    this.userForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(80)]],
      lastName: ['', [Validators.required, Validators.maxLength(80)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(255)]],
      username: ['', [Validators.required, Validators.maxLength(50)]],
      password: [''],
      roleId: [null, [Validators.required]],
      departmentId: [null, [Validators.required]],
      jobTitle: ['', [Validators.maxLength(120)]],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.loadReferenceData();
    this.loadUsers();
    this.loadSystemSettings();
  }

  showUsersSection(): void {
    this.activeSection = 'users';
    this.showUserForm = false;
    this.errorMessage = '';
    this.cdr.detectChanges();
  }

  showSettingsSection(): void {
    this.activeSection = 'settings';
    this.showUserForm = false;
    this.errorMessage = '';
    this.cdr.detectChanges();
  }

  loadReferenceData(): void {
    this.usersService.getRoles().subscribe({
      next: roles => {
        this.roles = roles;
        this.cdr.detectChanges();
      },
      error: err => {
        console.error('Failed to load roles', err);
        this.errorMessage = 'Failed to load roles.';
        this.cdr.detectChanges();
      }
    });

    this.usersService.getDepartments().subscribe({
      next: departments => {
        this.departments = departments;
        this.cdr.detectChanges();
      },
      error: err => {
        console.error('Failed to load departments', err);
        this.errorMessage = 'Failed to load departments.';
        this.cdr.detectChanges();
      }
    });
  }

  loadUsers(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.usersService.getUsers({
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm.trim() || undefined,
      roleId: this.roleFilter,
      isActive: this.getStatusFilterValue()
    }).subscribe({
      next: result => {
        this.users = result.items;
        this.pageNumber = result.pageNumber;
        this.pageSize = result.pageSize;
        this.totalCount = result.totalCount;
        this.totalPages = result.totalPages;
        this.hasPreviousPage = result.hasPreviousPage;
        this.hasNextPage = result.hasNextPage;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: err => {
        console.error('Failed to load users', err);
        this.errorMessage = 'Failed to load users.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadSystemSettings(): void {
    this.isLoadingSettings = true;
    this.settingsMessage = '';

    this.systemSettingsService.getSettings().subscribe({
      next: settings => {
        this.systemSettings = settings;
        this.settingValues = settings.reduce<Record<number, string>>((values, setting) => {
          values[setting.systemSettingId] = setting.settingValue;
          return values;
        }, {});
        this.isLoadingSettings = false;
        this.cdr.detectChanges();
      },
      error: err => {
        console.error('Failed to load system settings', err);
        this.errorMessage = 'Failed to load system settings.';
        this.isLoadingSettings = false;
        this.cdr.detectChanges();
      }
    });
  }

  applyFilters(): void {
    this.pageNumber = 1;
    this.loadUsers();
  }

  showFilters(): boolean {
    return this.filtersVisible;
  }

  toggleFilters(): void {
    this.filtersVisible = !this.filtersVisible;
  }

  clearFilters(): void {
    this.searchTerm = '';
    this.roleFilter = null;
    this.statusFilter = '';
    this.applyFilters();
  }

  openCreateForm(): void {
    this.selectedUser = null;
    this.userForm.reset({
      firstName: '',
      lastName: '',
      email: '',
      username: '',
      password: '',
      roleId: null,
      departmentId: null,
      jobTitle: '',
      isActive: true
    });
    this.userForm.get('password')?.setValidators([Validators.required, Validators.minLength(8)]);
    this.userForm.get('password')?.updateValueAndValidity();
    this.showUserForm = true;
    this.cdr.detectChanges();
  }

  openEditForm(user: User): void {
    this.selectedUser = user;
    this.userForm.reset({
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      username: user.username,
      password: '',
      roleId: user.roleId,
      departmentId: user.departmentId,
      jobTitle: user.jobTitle ?? '',
      isActive: user.isActive
    });
    this.userForm.get('password')?.setValidators([Validators.minLength(8)]);
    this.userForm.get('password')?.updateValueAndValidity();
    this.showUserForm = true;
    this.cdr.detectChanges();
  }

  closeUserForm(): void {
    this.showUserForm = false;
    this.selectedUser = null;
    this.isSaving = false;
    this.cdr.detectChanges();
  }

  saveUser(): void {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }

    const request = this.buildSaveRequest();
    this.isSaving = true;
    this.errorMessage = '';

    if (this.selectedUser) {
      this.usersService.update(this.selectedUser.userId, request).subscribe({
        next: () => this.onSaveSuccess(),
        error: (err: { error?: { message?: string } }) => this.onSaveError(err)
      });
      return;
    }

    this.usersService.create(request).subscribe({
      next: () => {
        this.onSaveSuccess();
      },
      error: (err: { error?: { message?: string } }) => this.onSaveError(err)
    });
  }

  saveSetting(setting: SystemSetting): void {
    const value = this.settingValues[setting.systemSettingId];
    const validationError = this.validateSettingValue(setting, value);

    if (validationError) {
      this.errorMessage = validationError;
      this.settingsMessage = '';
      this.cdr.detectChanges();
      return;
    }

    this.errorMessage = '';
    this.settingsMessage = '';
    this.savingSettingIds.add(setting.systemSettingId);

    this.systemSettingsService.update(setting.systemSettingId, { settingValue: value.trim() }).subscribe({
      next: () => {
        this.savingSettingIds.delete(setting.systemSettingId);
        this.settingsMessage = `${this.formatSettingName(setting.settingKey)} updated.`;
        this.loadSystemSettings();
      },
      error: (err: { error?: { message?: string } }) => {
        console.error('Failed to update system setting', err);
        this.savingSettingIds.delete(setting.systemSettingId);
        this.errorMessage = err?.error?.message ?? 'Failed to update system setting.';
        this.cdr.detectChanges();
      }
    });
  }

  goToPreviousPage(): void {
    if (!this.hasPreviousPage) {
      return;
    }

    this.pageNumber--;
    this.loadUsers();
  }

  goToNextPage(): void {
    if (!this.hasNextPage) {
      return;
    }

    this.pageNumber++;
    this.loadUsers();
  }

  onPageSizeChange(value: number): void {
    if (!this.pageSizeOptions.includes(value)) {
      return;
    }

    this.pageSize = value;
    this.pageNumber = 1;
    this.loadUsers();
  }

  getInitials(user: User): string {
    return `${user.firstName.charAt(0)}${user.lastName.charAt(0)}`.toUpperCase();
  }

  getRoleClass(role: string): string {
    if (role === 'Admin') {
      return 'app-badge app-badge--danger';
    }

    if (role === 'Operations') {
      return 'app-badge app-badge--info';
    }

    return 'app-badge app-badge--neutral';
  }

  getSettingsByGroup(group: string): SystemSetting[] {
    return this.systemSettings.filter(setting => this.getSettingGroup(setting.settingKey) === group);
  }

  getSettingGroups(): string[] {
    return ['Orders', 'Background Processing', 'Dashboard', 'Compliance'];
  }

  getSettingGroup(settingKey: string): string {
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

  private buildSaveRequest(): UserSaveRequest {
    const value = this.userForm.value;

    return {
      firstName: value.firstName,
      lastName: value.lastName,
      email: value.email,
      username: value.username,
      password: value.password || undefined,
      roleId: Number(value.roleId),
      departmentId: Number(value.departmentId),
      jobTitle: value.jobTitle || null,
      isActive: Boolean(value.isActive)
    };
  }

  private onSaveSuccess(): void {
    this.closeUserForm();
    this.loadUsers();
  }

  private onSaveError(err: { error?: { message?: string } }): void {
    console.error('Failed to save user', err);
    this.errorMessage = err?.error?.message ?? 'Failed to save user.';
    this.isSaving = false;
    this.cdr.detectChanges();
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

  private getStatusFilterValue(): boolean | null {
    if (this.statusFilter === 'active') {
      return true;
    }

    if (this.statusFilter === 'inactive') {
      return false;
    }

    return null;
  }
}
