import { Component, signal } from '@angular/core';
import { Router } from '@angular/router';
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
    private router: Router,
    public themeService: ThemeService
  ) {
    this.authService.currentUser$.subscribe(user => {
      this.currentRole.set(user?.role ?? 'User');
      this.userName.set(user?.fullName ?? 'User');
    });
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

  canViewAdmin(): boolean {
    return this.authService.hasRole('Admin', 'Demo');
  }

  canViewAudit(): boolean {
    return this.authService.hasRole('Admin', 'Demo');
  }

  logout(): void {
    this.authService.logout().subscribe(() => {
      this.router.navigateByUrl('/login', { replaceUrl: true });
    });
  }
}
