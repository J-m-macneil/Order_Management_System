import { Component, ViewChild } from '@angular/core';

import { AuthService } from '../../core/auth/auth.service';
import { AdminUsersComponent } from './admin-users/admin-users.component';

@Component({
  selector: 'app-admin',
  standalone: false,
  templateUrl: './admin.component.html'
})
export class AdminComponent {
  @ViewChild(AdminUsersComponent) private usersComponent?: AdminUsersComponent;

  activeSection: 'users' | 'settings' = 'users';
  isUserFormOpen = false;

  constructor(private authService: AuthService) { }

  get isDemoUser(): boolean {
    return this.authService.isDemoUser();
  }

  showUsersSection(): void {
    this.activeSection = 'users';
  }

  showSettingsSection(): void {
    this.activeSection = 'settings';
    this.isUserFormOpen = false;
  }

  openCreateUserForm(): void {
    this.usersComponent?.openCreateForm();
  }

  onUserFormModeChange(isOpen: boolean): void {
    this.isUserFormOpen = isOpen;
  }
}
