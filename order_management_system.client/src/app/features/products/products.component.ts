import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ProductList } from '../models/product-list.model';
import { ProductsService } from './products.service';

@Component({
  selector: 'app-products',
  standalone: false,
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css'],
})
export class ProductsComponent implements OnInit {
  products: ProductList[] = [];
  isLoading = false;
  errorMessage = '';

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

    this.productsService.getAll().subscribe({
      next: (data) => {
        this.products = data;
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

  deleteProduct(id: number): void {
    if (!confirm('Delete this product?')) {
      return;
    }

    this.productsService.delete(id).subscribe({
      next: () => {
        this.loadProducts();
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to delete product', err);
        this.errorMessage = 'Failed to delete product.';
        this.cdr.detectChanges();
      }
    });
  }
}
