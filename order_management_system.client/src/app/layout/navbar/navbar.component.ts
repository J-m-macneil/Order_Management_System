import { Component, signal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-navbar',
  standalone: false,
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
})
export class NavbarComponent {
  isDarkMode = signal(false);
  currentRole = signal<'sales' | 'operations' | 'admin'>('sales');
  userName = signal('John Doe');

  roles: Array<'sales' | 'operations' | 'admin'> = ['sales', 'operations', 'admin'];

  roleLabels: Record<string, string> = {
    sales: 'Sales',
    operations: 'Operations',
    admin: 'Admin'
  };

  roleIcons: Record<string, string> = {
    sales: '📊',
    operations: '⚙️',
    admin: '🔐'
  };

  constructor() {
    // Load dark mode preference from localStorage
    const savedDarkMode = localStorage.getItem('darkMode');
    if (savedDarkMode === 'true') {
      this.isDarkMode.set(true);
      document.documentElement.classList.add('dark');
    }

    // Load user role from localStorage
    const savedRole = localStorage.getItem('userRole') as 'sales' | 'operations' | 'admin' | null;
    if (savedRole && this.roles.includes(savedRole)) {
      this.currentRole.set(savedRole);
    }
  }

  toggleDarkMode() {
    const newDarkMode = !this.isDarkMode();
    this.isDarkMode.set(newDarkMode);

    if (newDarkMode) {
      document.documentElement.classList.add('dark');
    } else {
      document.documentElement.classList.remove('dark');
    }

    localStorage.setItem('darkMode', newDarkMode.toString());
  }

  switchRole(role: 'sales' | 'operations' | 'admin') {
    this.currentRole.set(role);
    localStorage.setItem('userRole', role);
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
