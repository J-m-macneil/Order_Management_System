import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

import { AuthService } from '../../../core/auth/auth.service';
import { SafetyDataSheet } from '../../../core/models/safety-data-sheet-model';
import { ProductsService } from '../../../core/services/products.service';
import { ToastService } from '../../../core/services/toast.service';
import { ApiErrorResponse, getApiErrorMessage } from '../../../core/utils/api-error-message';

@Component({
  selector: 'app-product-sds-panel',
  standalone: false,
  templateUrl: './product-sds-panel.component.html'
})
export class ProductSdsPanelComponent implements OnInit {
  @Input({ required: true }) productId!: number;
  @Input() requiresSds = false;

  @Output() sdsChanged = new EventEmitter<void>();

  safetyDataSheets: SafetyDataSheet[] = [];
  sdsPendingDelete: SafetyDataSheet | null = null;
  isLoading = false;
  isGenerating = false;
  errorMessage = '';

  constructor(
    private productsService: ProductsService,
    private authService: AuthService,
    private toastService: ToastService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadSafetyDataSheets();
  }

  get canManageSds(): boolean {
    return this.authService.hasRole('Operations', 'Admin');
  }

  generateSafetyDataSheet(): void {
    if (!this.requiresSds || !this.canManageSds || this.isGenerating) {
      return;
    }

    this.isGenerating = true;
    this.errorMessage = '';

    this.productsService.generateSafetyDataSheet(this.productId).subscribe({
      next: () => {
        this.isGenerating = false;
        this.toastService.success('SDS generated', 'The safety data sheet is ready to view.');
        this.loadSafetyDataSheets();
        this.sdsChanged.emit();
      },
      error: (error: ApiErrorResponse) => {
        console.error('Failed to generate SDS', error);
        this.errorMessage = getApiErrorMessage(error, 'Failed to generate SDS.');
        this.isGenerating = false;
        this.cdr.markForCheck();
      }
    });
  }

  getViewUrl(sds: SafetyDataSheet): string {
    return this.productsService.getSafetyDataSheetViewUrl(sds.productId, sds.safetyDataSheetId);
  }

  openDeleteModal(sds: SafetyDataSheet): void {
    if (this.canManageSds) {
      this.sdsPendingDelete = sds;
    }
  }

  cancelDelete(): void {
    this.sdsPendingDelete = null;
  }

  confirmDelete(): void {
    if (!this.sdsPendingDelete || !this.canManageSds) {
      return;
    }

    const sdsId = this.sdsPendingDelete.safetyDataSheetId;
    this.errorMessage = '';

    this.productsService.deleteSafetyDataSheet(this.productId, sdsId).subscribe({
      next: () => {
        this.sdsPendingDelete = null;
        this.toastService.success('SDS deleted', 'The safety data sheet was removed.');
        this.loadSafetyDataSheets();
        this.sdsChanged.emit();
      },
      error: (error: ApiErrorResponse) => {
        console.error('Failed to delete SDS', error);
        this.errorMessage = getApiErrorMessage(error, 'Failed to delete SDS.');
        this.sdsPendingDelete = null;
        this.cdr.markForCheck();
      }
    });
  }

  private loadSafetyDataSheets(): void {
    this.isLoading = true;

    this.productsService.getSafetyDataSheets(this.productId).subscribe({
      next: data => {
        this.safetyDataSheets = data;
        this.errorMessage = '';
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: error => {
        console.error('Failed to load SDS records', error);
        this.errorMessage = 'Failed to load SDS records.';
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }
}
