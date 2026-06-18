import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { Department, Role, User, UserSaveRequest } from '../../core/models/user-management.model';
import { UsersService } from '../../core/services/users.service';

@Component({
  selector: 'app-admin',
  standalone: false,
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
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
  pageSizeOptions = [25, 50, 100];

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
      return 'bg-red-100 dark:bg-red-900/20 text-red-700 dark:text-red-400';
    }

    if (role === 'Operations') {
      return 'bg-purple-100 dark:bg-purple-900/20 text-purple-700 dark:text-purple-400';
    }

    return 'bg-blue-100 dark:bg-blue-900/20 text-blue-700 dark:text-blue-400';
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
