import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ProductList } from '../../core/models/product-list.model';
import { ProductsService } from '../../core/services/products.service';
import { ProductCategory } from '../../core/models/product-category.model';
import { HazardClass } from '../../core/models/hazard-class.model';

@Component({
  selector: 'app-products',
  standalone: false,
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css'],
})
export class ProductsComponent implements OnInit {
  products: ProductList[] = [];
  filteredProducts: ProductList[] = [];

  pageNumber = 1;
  pageSize = 25;
  totalCount = 0;
  totalPages = 0;
  hasPreviousPage = false;
  hasNextPage = false;
  pageSizeOptions = [25, 50, 100];

  isLoading = false;
  errorMessage = '';

  searchTerm = '';
  activeFilter = '';
  restrictedFilter = '';
  hazardousFilter = '';
  categoryFilter: number | null = null;
  hazardClassFilter: number | null = null;

  private filtersVisible = false;

  stats: { label: string; value: string | number }[] = [];
  categories: ProductCategory[] = [];
  hazardClasses: HazardClass[] = [];
  productPendingDelete: ProductList | null = null;

  private isHazardous(product: ProductList): boolean {
    return product.hazardClassName?.toLowerCase() !== 'non-hazardous';
  }

  constructor(
    private productsService: ProductsService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadSummary();
    this.loadFilterOptions();
    this.loadProducts();
  }

  loadFilterOptions(): void {
    this.productsService.getProductCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
        this.cdr.detectChanges();
      }
    });

    this.productsService.getHazardClasses().subscribe({
      next: (hazardClasses) => {
        this.hazardClasses = hazardClasses;
        this.cdr.detectChanges();
      }
    });
  }

  loadSummary(): void {
    this.productsService.getSummary().subscribe({
      next: (summary) => {
        this.stats = [
          {
            label: 'Total Products',
            value: summary.totalProducts
          },
          {
            label: 'Active Products',
            value: summary.activeProducts
          },
          {
            label: 'Restricted',
            value: summary.restrictedProducts
          },
          {
            label: 'Hazardous',
            value: summary.hazardousProducts
          }
        ];
        this.cdr.detectChanges();
      }
    });
  }

  loadProducts(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.productsService.getAll({
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm.trim() || undefined,
      isActive: this.getActiveFilterValue(),
      isRestricted: this.getRestrictedFilterValue(),
      isHazardous: this.getHazardousFilterValue(),
      productCategoryId: this.categoryFilter,
      hazardClassId: this.hazardClassFilter
    })
    .subscribe({
      next: (data) => {
        this.products = data.items;
        this.pageNumber = data.pageNumber;
        this.pageSize = data.pageSize;
        this.totalCount = data.totalCount;
        this.totalPages = data.totalPages;
        this.hasPreviousPage = data.hasPreviousPage;
        this.hasNextPage = data.hasNextPage;
        this.initialiseProductDashboard();
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load products', err);
        this.errorMessage = 'Failed to load products.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  showFilters(): boolean {
    return this.filtersVisible;
  }

  toggleFilters(): void {
    this.filtersVisible = !this.filtersVisible;
  }

  applyFilters(): void {
    this.pageNumber = 1;
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

  private initialiseProductDashboard(): void {
    this.filteredProducts = this.products;
  }

  openDeleteProductModal(product: ProductList): void {
    this.productPendingDelete = product;
  }

  cancelDeleteProduct(): void {
    this.productPendingDelete = null;
  }

  confirmDeleteProduct(): void {
    if (!this.productPendingDelete) {
      return;
    }

    this.productsService.delete(this.productPendingDelete.productId).subscribe({
      next: () => {
        this.productPendingDelete = null;
        this.loadSummary();
        this.loadProducts();
      },
      error: (err) => {
        console.error('Failed to delete product', err);
        this.errorMessage = 'Failed to delete product.';
        this.productPendingDelete = null;
        this.cdr.detectChanges();
      }
    });
  }

  goToPreviousPage(): void {
    if (!this.hasPreviousPage) {
      return;
    }

    this.pageNumber--;
    this.loadProducts();
  }

  goToNextPage(): void {
    if (!this.hasNextPage) {
      return;
    }

    this.pageNumber++;
    this.loadProducts();
  }

  onPageSizeChange(value: number): void {
    if (!this.pageSizeOptions.includes(value)) {
      return;
    }

    this.pageSize = value;
    this.pageNumber = 1;
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
}
