import { Component, signal } from '@angular/core';
import { AuthService } from '../../core/auth/auth.service';
import { ThemeService } from '../../core/services/theme.service';

@Component({
  selector: 'app-navbar',
  standalone: false,
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
})
export class NavbarComponent {
  currentRole = signal('User');
  userName = signal('User');

  constructor(
    private authService: AuthService,
    public themeService: ThemeService
  ) {
    this.loadUserFromToken();
  }

  private loadUserFromToken(): void {
    const role = this.authService.getUserRole();
    const name = this.authService.getUserFullName();

    this.currentRole.set(role ?? 'User');
    this.userName.set(name ?? 'User');
  }

  toggleDarkMode(): void {
    this.themeService.toggleDarkMode();
  }

  getUserInitials(): string {
    return this.userName()
      .split(' ')
      .map(n => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  }
}
