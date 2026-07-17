import { FormBuilder } from '@angular/forms';
import { of } from 'rxjs';

import { PagedResult } from '../../../core/models/paged-result.model';
import { User } from '../../../core/models/user-management.model';
import { UsersService } from '../../../core/services/users.service';
import { ToastService } from '../../../core/services/toast.service';
import { AdminUsersComponent } from './admin-users.component';

describe('AdminUsersComponent', () => {
  it('clears advanced filters without clearing search', () => {
    const requests: Record<string, unknown>[] = [];
    const result: PagedResult<User> = {
      items: [],
      pageNumber: 1,
      pageSize: 25,
      totalCount: 0,
      totalPages: 0,
      hasPreviousPage: false,
      hasNextPage: false
    };
    const service = {
      getUsers: (query: Record<string, unknown>) => {
        requests.push(query);
        return of(result);
      }
    } as unknown as UsersService;
    const toastService = { success: () => undefined } as unknown as ToastService;
    const component = new AdminUsersComponent(service, new FormBuilder(), toastService);

    component.searchTerm = 'alex';
    component.roleFilter = 2;
    component.statusFilter = 'inactive';
    component.pageNumber.set(3);

    component.clearFilters();

    expect(component.searchTerm).toBe('alex');
    expect(component.roleFilter).toBeNull();
    expect(component.statusFilter).toBe('');
    expect(requests[0]['pageNumber']).toBe(1);
  });
});
