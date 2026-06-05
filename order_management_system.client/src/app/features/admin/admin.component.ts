import { Component, OnInit } from '@angular/core';

interface AdminUser {
  name: string;
  username: string;
  email: string;
  role: string;
  initials: string;
  avatarClass: string;
  roleClass: string;
  isActive: boolean;
}

@Component({
  selector: 'app-admin',
  standalone: false,
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {
  users: AdminUser[] = [
    {
      name: 'John Doe',
      username: 'johndoe',
      email: 'john@example.com',
      role: 'Sales',
      initials: 'JD',
      avatarClass: 'bg-blue-600',
      roleClass: 'bg-blue-100 dark:bg-blue-900/20 text-blue-700 dark:text-blue-400',
      isActive: true
    },
    {
      name: 'Sarah Miller',
      username: 'sarahmiller',
      email: 'sarah@example.com',
      role: 'Operations',
      initials: 'SM',
      avatarClass: 'bg-purple-600',
      roleClass: 'bg-purple-100 dark:bg-purple-900/20 text-purple-700 dark:text-purple-400',
      isActive: true
    },
    {
      name: 'Admin Brown',
      username: 'adminbrown',
      email: 'admin@example.com',
      role: 'Admin',
      initials: 'AB',
      avatarClass: 'bg-red-600',
      roleClass: 'bg-red-100 dark:bg-red-900/20 text-red-700 dark:text-red-400',
      isActive: true
    }
  ];

  pagedUsers: AdminUser[] = [];

  pageNumber = 1;
  pageSize = 25;
  totalCount = 0;
  totalPages = 0;
  hasPreviousPage = false;
  hasNextPage = false;
  pageSizeOptions = [25, 50, 100];

  ngOnInit(): void {
    this.updatePagedUsers();
  }

  goToPreviousPage(): void {
    if (!this.hasPreviousPage) {
      return;
    }

    this.pageNumber--;
    this.updatePagedUsers();
  }

  goToNextPage(): void {
    if (!this.hasNextPage) {
      return;
    }

    this.pageNumber++;
    this.updatePagedUsers();
  }

  onPageSizeChange(value: number): void {
    if (!this.pageSizeOptions.includes(value)) {
      return;
    }

    this.pageSize = value;
    this.pageNumber = 1;
    this.updatePagedUsers();
  }

  private updatePagedUsers(): void {
    this.totalCount = this.users.length;
    this.totalPages = Math.ceil(this.totalCount / this.pageSize);

    const skip = (this.pageNumber - 1) * this.pageSize;
    this.pagedUsers = this.users.slice(skip, skip + this.pageSize);

    this.hasPreviousPage = this.pageNumber > 1;
    this.hasNextPage = this.pageNumber < this.totalPages;
  }
}
