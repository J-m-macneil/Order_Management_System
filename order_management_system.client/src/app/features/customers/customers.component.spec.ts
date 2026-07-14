import { ChangeDetectorRef } from '@angular/core';
import { of } from 'rxjs';

import { Customer } from '../../core/models/customer.model';
import { PagedResult } from '../../core/models/paged-result.model';
import { CustomersService } from '../../core/services/customers.service';
import { CustomersComponent } from './customers.component';

describe('CustomersComponent', () => {
  let component: CustomersComponent;
  let requests: Record<string, unknown>[];

  beforeEach(() => {
    requests = [];

    const result: PagedResult<Customer> = {
      items: [],
      pageNumber: 1,
      pageSize: 25,
      totalCount: 0,
      totalPages: 0,
      hasPreviousPage: false,
      hasNextPage: false
    };

    const service = {
      getAll: (query: Record<string, unknown>) => {
        requests.push(query);
        return of(result);
      },
      getSummary: () => of({ totalCustomers: 0, activeCustomers: 0, inactiveCustomers: 0 }),
      delete: () => of(void 0)
    } as unknown as CustomersService;

    const cdr = { detectChanges: () => undefined } as unknown as ChangeDetectorRef;
    component = new CustomersComponent(service, cdr);
  });

  it('clears advanced filters without clearing search', () => {
    component.pageNumber = 3;
    component.searchTerm = 'Acme';
    component.industryFilter = 'Manufacturing';
    component.paymentTermsFilter = '30';
    component.activeFilter = 'active';

    component.clearFilters();

    expect(component.searchTerm).toBe('Acme');
    expect(component.industryFilter).toBe('');
    expect(component.paymentTermsFilter).toBe('');
    expect(component.activeFilter).toBe('');
    expect(requests[0]['pageNumber']).toBe(1);
  });

  it('returns to the previous page after deleting its final customer', () => {
    const customer = { customerId: 10 } as Customer;
    component.customers = [customer];
    component.customerPendingDelete = customer;
    component.pageNumber = 2;

    component.confirmDeleteCustomer();

    expect(component.pageNumber).toBe(1);
    expect(requests[0]['pageNumber']).toBe(1);
  });
});
