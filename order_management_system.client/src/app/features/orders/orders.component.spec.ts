import { of } from 'rxjs';
import { ActivatedRoute, convertToParamMap, Router } from '@angular/router';

import { PagedResult } from '../../core/models/paged-result.model';
import { Order } from '../../core/models/order.model';
import { OrdersService } from '../../core/services/orders.service';
import { OrdersComponent } from './orders.component';

describe('OrdersComponent', () => {
  let component: OrdersComponent;
  let requests: Record<string, unknown>[];

  beforeEach(() => {
    requests = [];

    const result: PagedResult<Order> = {
      items: [],
      pageNumber: 1,
      pageSize: 25,
      totalCount: 0,
      totalPages: 0,
      hasPreviousPage: false,
      hasNextPage: false
    };

    const service = {
      getOrders: (query: Record<string, unknown>) => {
        requests.push(query);
        return of(result);
      }
    } as unknown as OrdersService;

    const route = {
      snapshot: { queryParamMap: convertToParamMap({}) }
    } as unknown as ActivatedRoute;
    const router = {
      navigate: () => Promise.resolve(true)
    } as unknown as Router;

    component = new OrdersComponent(service, route, router);
  });

  it('resets pagination and trims search when filters are applied', () => {
    component.pageNumber.set(3);
    component.searchTerm = '  ORD-1001  ';

    component.applyFilters();

    expect(component.pageNumber()).toBe(1);
    expect(requests[0]['searchTerm']).toBe('ORD-1001');
  });

  it('preserves the standard priority filter value in the request', () => {
    component.priorityFilter = 'standard';

    component.applyFilters();

    expect(requests[0]['isPriorityOrder']).toBe(false);
  });

  it('loads the requested page and resets to page one when page size changes', () => {
    component.onPageChange(2);
    expect(requests[0]['pageNumber']).toBe(2);

    component.onPageSizeChange(50);
    expect(requests[1]['pageNumber']).toBe(1);
    expect(requests[1]['pageSize']).toBe(50);
  });
});
