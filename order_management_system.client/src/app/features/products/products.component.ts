import { Component, OnInit, signal } from '@angular/core';
import { ProductList } from '../../core/models/product-list.model';
import { ProductsService } from '../../core/services/products.service';
import { ProductCategory } from '../../core/models/product-category.model';
import { HazardClass } from '../../core/models/hazard-class.model';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-products',
  standalone: false,
  templateUrl: './products.component.html'
})
export class ProductsComponent implements OnInit {
  readonly products = signal<ProductList[]>([]);

  readonly pageNumber = signal(1);
  readonly pageSize = signal(25);
  readonly totalCount = signal(0);
  readonly totalPages = signal(0);
  readonly hasPreviousPage = signal(false);
  readonly hasNextPage = signal(false);

  readonly isLoading = signal(false);
  readonly errorMessage = signal('');

  searchTerm = '';
  activeFilter = '';
  restrictedFilter = '';
  hazardousFilter = '';
  categoryFilter: number | null = null;
  hazardClassFilter: number | null = null;

  filtersVisible = false;

  readonly stats = signal<{
    label: string;
    value: string | number;
    type: 'total' | 'active' | 'restricted' | 'hazardous';
    description: string;
  }[]>([]);
  readonly categories = signal<ProductCategory[]>([]);
  readonly hazardClasses = signal<HazardClass[]>([]);
  readonly productPendingDelete = signal<ProductList | null>(null);

  constructor(
    private productsService: ProductsService,
    private toastService: ToastService
  ) { }

  ngOnInit(): void {
    this.loadSummary();
    this.loadFilterOptions();
    this.loadProducts();
  }

  loadFilterOptions(): void {
    this.productsService.getProductCategories().subscribe({
      next: (categories) => {
        this.categories.set(categories);
      }
    });

    this.productsService.getHazardClasses().subscribe({
      next: (hazardClasses) => {
        this.hazardClasses.set(hazardClasses);
      }
    });
  }

  loadSummary(): void {
    this.productsService.getSummary().subscribe({
      next: (summary) => {
        this.stats.set([
          {
            label: 'Total Products',
            value: summary.totalProducts,
            type: 'total',
            description: 'Products in the catalogue'
          },
          {
            label: 'Active Products',
            value: summary.activeProducts,
            type: 'active',
            description: 'Available to order'
          },
          {
            label: 'Restricted',
            value: summary.restrictedProducts,
            type: 'restricted',
            description: 'Require additional controls'
          },
          {
            label: 'Hazardous',
            value: summary.hazardousProducts,
            type: 'hazardous',
            description: 'Classified with a hazard'
          }
        ]);
      }
    });
  }

  loadProducts(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.productsService.getAll({
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      searchTerm: this.searchTerm.trim() || undefined,
      isActive: this.getActiveFilterValue(),
      isRestricted: this.getRestrictedFilterValue(),
      isHazardous: this.getHazardousFilterValue(),
      productCategoryId: this.categoryFilter,
      hazardClassId: this.hazardClassFilter
    })
    .subscribe({
      next: (data) => {
        this.products.set(data.items);
        this.pageNumber.set(data.pageNumber);
        this.pageSize.set(data.pageSize);
        this.totalCount.set(data.totalCount);
        this.totalPages.set(data.totalPages);
        this.hasPreviousPage.set(data.hasPreviousPage);
        this.hasNextPage.set(data.hasNextPage);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Failed to load products', err);
        this.errorMessage.set('Failed to load products.');
        this.isLoading.set(false);
      }
    });
  }

  toggleFilters(): void {
    this.filtersVisible = !this.filtersVisible;
  }

  applyFilters(): void {
    this.pageNumber.set(1);
    this.loadProducts();
  }

  clearFilters(): void {
    this.activeFilter = '';
    this.restrictedFilter = '';
    this.hazardousFilter = '';
    this.categoryFilter = null;
    this.hazardClassFilter = null;
    this.applyFilters();
  }

  openDeleteProductModal(product: ProductList): void {
    this.productPendingDelete.set(product);
  }

  cancelDeleteProduct(): void {
    this.productPendingDelete.set(null);
  }

  confirmDeleteProduct(): void {
    const product = this.productPendingDelete();
    if (!product) {
      return;
    }

    const moveToPreviousPage = this.products().length === 1 && this.pageNumber() > 1;

    this.productsService.delete(product.productId).subscribe({
      next: () => {
        this.productPendingDelete.set(null);
        this.toastService.success('Product deleted', `${product.productName} was removed.`);

        if (moveToPreviousPage) {
          this.pageNumber.update(page => page - 1);
        }

        this.loadSummary();
        this.loadProducts();
      },
      error: (err) => {
        console.error('Failed to delete product', err);
        this.toastService.error('Product deletion failed', 'The product could not be deleted.');
        this.productPendingDelete.set(null);
      }
    });
  }

  onPageChange(pageNumber: number): void {
    this.pageNumber.set(pageNumber);
    this.loadProducts();
  }

  onPageSizeChange(value: number): void {
    this.pageSize.set(value);
    this.pageNumber.set(1);
    this.loadProducts();
  }

  private getActiveFilterValue(): boolean | null {
    if (this.activeFilter === 'active') {
      return true;
    }

    if (this.activeFilter === 'inactive') {
      return false;
    }

    return null;
  }

  private getRestrictedFilterValue(): boolean | null {
    if (this.restrictedFilter === 'restricted') {
      return true;
    }

    if (this.restrictedFilter === 'unrestricted') {
      return false;
    }

    return null;
  }

  private getHazardousFilterValue(): boolean | null {
    if (this.hazardousFilter === 'hazardous') {
      return true;
    }

    if (this.hazardousFilter === 'nonhazardous') {
      return false;
    }

    return null;
  }

  getHazardClassBadge(hazardClassName: string): string {
    return hazardClassName && hazardClassName !== 'Non-Hazardous'
      ? 'app-badge app-badge--warning'
      : 'app-badge app-badge--neutral';
  }

  get activeFilterCount(): number {
    return [
      this.activeFilter,
      this.restrictedFilter,
      this.hazardousFilter,
      this.categoryFilter,
      this.hazardClassFilter
    ].filter(Boolean).length;
  }
}
