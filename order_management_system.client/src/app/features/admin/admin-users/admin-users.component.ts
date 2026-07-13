import { ChangeDetectorRef, Component, EventEmitter, OnInit, Output } from '@angular/core';
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

  users: User[] = [];
  roles: Role[] = [];
  departments: Department[] = [];

  userForm: FormGroup;
  selectedUser: User | null = null;
  showUserForm = false;

  pageNumber = 1;
  pageSize = 25;
  totalCount = 0;
  totalPages = 0;
  hasPreviousPage = false;
  hasNextPage = false;
  readonly pageSizeOptions = [25, 50, 100];

  searchTerm = '';
  roleFilter: number | null = null;
  statusFilter = '';
  filtersVisible = false;

  isLoading = false;
  isSaving = false;
  errorMessage = '';

  constructor(
    private usersService: UsersService,
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

  applyFilters(): void {
    this.pageNumber = 1;
    this.loadUsers();
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

  get activeFilterCount(): number {
    return [
      this.roleFilter,
      this.statusFilter
    ].filter(Boolean).length;
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
    this.formModeChange.emit(true);
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
    this.formModeChange.emit(true);
    this.cdr.detectChanges();
  }

  closeUserForm(): void {
    this.showUserForm = false;
    this.selectedUser = null;
    this.isSaving = false;
    this.formModeChange.emit(false);
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

    const saveOperation: Observable<unknown> = this.selectedUser
      ? this.usersService.update(this.selectedUser.userId, request)
      : this.usersService.create(request);

    saveOperation.subscribe({
      next: () => this.onSaveSuccess(),
      error: (err: ApiErrorResponse) => this.onSaveError(err)
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

  getRoleClass(role: string): string {
    if (role === 'Admin') {
      return 'app-badge app-badge--danger';
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
    this.errorMessage = getApiErrorMessage(err, 'Failed to save user.');
    this.isSaving = false;
    this.cdr.detectChanges();
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
