export interface User {
  userId: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  roleId: number;
  role: string;
  departmentId: number;
  department: string;
  jobTitle?: string | null;
  isActive: boolean;
  createdAt: string;
  lastLoginAt?: string | null;
}

export interface Role {
  roleId: number;
  name: string;
}

export interface Department {
  departmentId: number;
  name: string;
}

export interface UserSaveRequest {
  firstName: string;
  lastName: string;
  email: string;
  username: string;
  password?: string;
  roleId: number;
  departmentId: number;
  jobTitle?: string | null;
  isActive: boolean;
}
