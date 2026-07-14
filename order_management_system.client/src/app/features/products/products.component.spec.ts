import { of } from 'rxjs';

import { PagedResult } from '../../core/models/paged-result.model';
import { ProductList } from '../../core/models/product-list.model';
import { ProductsService } from '../../core/services/products.service';
import { ProductsComponent } from './products.component';

describe('ProductsComponent', () => {
  let component: ProductsComponent;
  let requests: Record<string, unknown>[];

  beforeEach(() => {
    requests = [];

    const result: PagedResult<ProductList> = {
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
      getSummary: () => of({ totalProducts: 0, activeProducts: 0, restrictedProducts: 0, hazardousProducts: 0 }),
      delete: () => of(void 0)
    } as unknown as ProductsService;

    component = new ProductsComponent(service);
  });

  it('preserves false boolean filter values in the request', () => {
    component.activeFilter = 'inactive';
    component.restrictedFilter = 'unrestricted';
    component.hazardousFilter = 'nonhazardous';

    component.applyFilters();

    expect(requests[0]['isActive']).toBe(false);
    expect(requests[0]['isRestricted']).toBe(false);
    expect(requests[0]['isHazardous']).toBe(false);
  });

  it('returns to the previous page after deleting its final product', () => {
    const product = { productId: 10 } as ProductList;
    component.products.set([product]);
    component.productPendingDelete.set(product);
    component.pageNumber.set(2);

    component.confirmDeleteProduct();

    expect(component.pageNumber()).toBe(1);
    expect(requests[0]['pageNumber']).toBe(1);
  });
});
