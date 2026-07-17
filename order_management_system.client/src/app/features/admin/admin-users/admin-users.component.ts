import { Component, EventEmitter, OnInit, Output, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable } from 'rxjs';

import { Department, Role, User, UserSaveRequest } from '../../../core/models/user-management.model';
import { UsersService } from '../../../core/services/users.service';
import { ApiErrorResponse, getApiErrorMessage } from '../../../core/utils/api-error-message';

@Component({
  selector: 'app-admin-users',
  standalone: false,
  templateUrl: './admin-users.component.html'
})
export class AdminUsersComponent implements OnInit {
  @Output() formModeChange = new EventEmitter<boolean>();

  readonly users = signal<User[]>([]);
  readonly roles = signal<Role[]>([]);
  readonly departments = signal<Department[]>([]);

  userForm: FormGroup;
  readonly selectedUser = signal<User | null>(null);
  readonly showUserForm = signal(false);

  readonly pageNumber = signal(1);
  readonly pageSize = signal(25);
  readonly totalCount = signal(0);
  readonly totalPages = signal(0);
  readonly hasPreviousPage = signal(false);
  readonly hasNextPage = signal(false);

  searchTerm = '';
  roleFilter: number | null = null;
  statusFilter = '';
  filtersVisible = false;

  readonly isLoading = signal(false);
  readonly userLoadFailed = signal(false);
  readonly isSaving = signal(false);
  readonly errorMessage = signal('');

  constructor(
    private usersService: UsersService,
    private fb: FormBuilder
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
  }

  loadReferenceData(): void {
    this.usersService.getRoles().subscribe({
      next: roles => {
        this.roles.set(roles);
      },
      error: err => {
        console.error('Failed to load roles', err);
        this.errorMessage.set('Failed to load roles.');
      }
    });

    this.usersService.getDepartments().subscribe({
      next: departments => {
        this.departments.set(departments);
      },
      error: err => {
        console.error('Failed to load departments', err);
        this.errorMessage.set('Failed to load departments.');
      }
    });
  }

  loadUsers(): void {
    this.isLoading.set(true);
    this.userLoadFailed.set(false);
    this.errorMessage.set('');

    this.usersService.getUsers({
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      searchTerm: this.searchTerm.trim() || undefined,
      roleId: this.roleFilter,
      isActive: this.getStatusFilterValue()
    }).subscribe({
      next: result => {
        this.users.set(result.items);
        this.pageNumber.set(result.pageNumber);
        this.pageSize.set(result.pageSize);
        this.totalCount.set(result.totalCount);
        this.totalPages.set(result.totalPages);
        this.hasPreviousPage.set(result.hasPreviousPage);
        this.hasNextPage.set(result.hasNextPage);
        this.isLoading.set(false);
      },
      error: err => {
        console.error('Failed to load users', err);
        this.errorMessage.set('Failed to load users.');
        this.userLoadFailed.set(true);
        this.isLoading.set(false);
      }
    });
  }

  applyFilters(): void {
    this.pageNumber.set(1);
    this.loadUsers();
  }

  toggleFilters(): void {
    this.filtersVisible = !this.filtersVisible;
  }

  clearFilters(): void {
    this.roleFilter = null;
    this.statusFilter = '';
    this.applyFilters();
  }

  get activeFilterCount(): number {
    return [
      this.roleFilter,
      this.statusFilter
    ].filter(Boolean).length;
  }

  openCreateForm(): void {
    this.selectedUser.set(null);
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
    this.showUserForm.set(true);
    this.formModeChange.emit(true);
  }

  openEditForm(user: User): void {
    this.selectedUser.set(user);
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
    this.showUserForm.set(true);
    this.formModeChange.emit(true);
  }

  closeUserForm(): void {
    this.showUserForm.set(false);
    this.selectedUser.set(null);
    this.isSaving.set(false);
    this.formModeChange.emit(false);
  }

  saveUser(): void {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }

    const request = this.buildSaveRequest();
    this.isSaving.set(true);
    this.errorMessage.set('');

    const selectedUser = this.selectedUser();
    const saveOperation: Observable<unknown> = selectedUser
      ? this.usersService.update(selectedUser.userId, request)
      : this.usersService.create(request);

    saveOperation.subscribe({
      next: () => this.onSaveSuccess(),
      error: (err: ApiErrorResponse) => this.onSaveError(err)
    });
  }

  onPageChange(pageNumber: number): void {
    this.pageNumber.set(pageNumber);
    this.loadUsers();
  }

  onPageSizeChange(value: number): void {
    this.pageSize.set(value);
    this.pageNumber.set(1);
    this.loadUsers();
  }

  getRoleClass(role: string): string {
    if (role === 'Admin') {
      return 'app-badge app-badge--info';
    }

    if (role === 'Operations') {
      return 'app-badge app-badge--info';
    }

    return 'app-badge app-badge--neutral';
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

  private onSaveError(err: ApiErrorResponse): void {
    console.error('Failed to save user', err);
    this.errorMessage.set(getApiErrorMessage(err, 'Failed to save user.'));
    this.isSaving.set(false);
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
