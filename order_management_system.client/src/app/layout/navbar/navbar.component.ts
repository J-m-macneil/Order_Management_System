import { Component, signal } from '@angular/core';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: false,
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
})
export class NavbarComponent {
  isDarkMode = signal(false);
  currentRole = signal('User');
  userName = signal('User');

  constructor(private authService: AuthService) {
    const savedDarkMode = localStorage.getItem('darkMode');

    if (savedDarkMode === 'true') {
      this.isDarkMode.set(true);
      document.documentElement.classList.add('dark');
    }

    this.loadUserFromToken();
  }

  private loadUserFromToken(): void {
    const role = this.authService.getUserRole();
    const name = this.authService.getUserFullName();

    this.currentRole.set(role ?? 'User');
    this.userName.set(name ?? 'User');
  }

  toggleDarkMode(): void {
    const newDarkMode = !this.isDarkMode();
    this.isDarkMode.set(newDarkMode);

    if (newDarkMode) {
      document.documentElement.classList.add('dark');
    } else {
      document.documentElement.classList.remove('dark');
    }

    localStorage.setItem('darkMode', newDarkMode.toString());
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
