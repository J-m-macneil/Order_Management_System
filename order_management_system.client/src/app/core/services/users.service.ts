import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { PagedResult } from '../models/paged-result.model';
import { Department, Role, User, UserSaveRequest } from '../models/user-management.model';

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  private readonly baseUrl = 'https://localhost:7233/api/users';

  constructor(private http: HttpClient) { }

  getUsers(query: {
    pageNumber: number;
    pageSize: number;
    searchTerm?: string;
    roleId?: number | null;
    isActive?: boolean | null;
  }): Observable<PagedResult<User>> {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize);

    if (query.searchTerm) {
      params = params.set('searchTerm', query.searchTerm);
    }

    if (query.roleId) {
      params = params.set('roleId', query.roleId);
    }

    if (query.isActive !== undefined && query.isActive !== null) {
      params = params.set('isActive', query.isActive);
    }

    return this.http.get<PagedResult<User>>(this.baseUrl, { params });
  }

  getRoles(): Observable<Role[]> {
    return this.http.get<Role[]>(`${this.baseUrl}/roles`);
  }

  getDepartments(): Observable<Department[]> {
    return this.http.get<Department[]>(`${this.baseUrl}/departments`);
  }

  create(request: UserSaveRequest): Observable<User> {
    return this.http.post<User>(this.baseUrl, request);
  }

  update(userId: number, request: UserSaveRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${userId}`, request);
  }
}
