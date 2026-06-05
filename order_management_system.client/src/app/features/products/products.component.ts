import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ProductList } from '../../core/models/product-list.model';
import { ProductsService } from '../../core/services/products.service';

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

  private filtersVisible = false;

  stats: { label: string; value: string | number; color: string }[] = [];

  private isHazardous(product: ProductList): boolean {
    return product.hazardClassName?.toLowerCase() !== 'non-hazardous';
  }

  constructor(
    private productsService: ProductsService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.productsService.getAll({
      pageNumber: this.pageNumber,
      pageSize: this.pageSize
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
    const term = this.searchTerm.toLowerCase().trim();

    this.filteredProducts = this.products.filter(product => {
      const matchesSearch =
        !term ||
        product.sku?.toLowerCase().includes(term) ||
        product.productName?.toLowerCase().includes(term) ||
        product.packSize?.toLowerCase().includes(term) ||
        product.productCategoryName?.toLowerCase().includes(term) ||
        product.unitOfMeasureName?.toLowerCase().includes(term) ||
        product.hazardClassName?.toLowerCase().includes(term);

      const matchesActive =
        !this.activeFilter ||
        (this.activeFilter === 'active' && product.isActive) ||
        (this.activeFilter === 'inactive' && !product.isActive);

      const matchesRestricted =
        !this.restrictedFilter ||
        (this.restrictedFilter === 'restricted' && product.isRestricted) ||
        (this.restrictedFilter === 'unrestricted' && !product.isRestricted);

      const isHazardous =
        product.hazardClassName?.toLowerCase() !== 'non-hazardous';

      const matchesHazardous =
        !this.hazardousFilter ||
        (this.hazardousFilter === 'hazardous' && isHazardous) ||
        (this.hazardousFilter === 'nonhazardous' && !isHazardous);

      return matchesSearch && matchesActive && matchesRestricted && matchesHazardous;
    });
  }

  private initialiseProductDashboard(): void {
    this.updateStats();
    this.applyFilters();
  }

  private updateStats(): void {
    const activeProducts = this.products.filter(p => p.isActive).length;

    const restrictedProducts = this.products.filter(p => p.isRestricted).length;

    const hazardousProducts = this.products.filter(p =>
      p.hazardClassName?.toLowerCase() !== 'non-hazardous'
    ).length;

    this.stats = [
      {
        label: 'Total Products',
        value: this.products.length,
        color: ''
      },
      {
        label: 'Active Products',
        value: activeProducts,
        color: 'text-emerald-600 dark:text-emerald-400'
      },
      {
        label: 'Restricted',
        value: restrictedProducts,
        color: 'text-red-600 dark:text-red-400'
      },
      {
        label: 'Hazardous',
        value: hazardousProducts,
        color: 'text-amber-600 dark:text-amber-400'
      }
    ];
  }

  deleteProduct(id: number): void {
    if (!confirm('Delete this product?')) {
      return;
    }

    this.productsService.delete(id).subscribe({
      next: () => {
        this.loadProducts();
      },
      error: (err) => {
        console.error('Failed to delete product', err);
        this.errorMessage = 'Failed to delete product.';
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
}
