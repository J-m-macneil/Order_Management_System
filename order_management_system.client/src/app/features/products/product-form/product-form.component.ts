import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HazardClass } from '../../../core/models/hazard-class.model';
import { ProductCategory } from '../../../core/models/product-category.model';
import { Product } from '../../../core/models/product.model';
import { SafetyDataSheet, CreateSafetyDataSheetRequest } from '../../../core/models/safety-data-sheet-model';
import { UnitOfMeasure } from '../../../core/models/unit-of-measure.model';
import { ProductsService } from '../../../core/services/products.service';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.css'],
  standalone: false
})
export class ProductFormComponent implements OnInit {
  form!: FormGroup;

  isEditMode = false;
  productId: number | null = null;
  isLoading = false;
  errorMessage = '';

  productCategories: ProductCategory[] = [];
  unitsOfMeasure: UnitOfMeasure[] = [];
  hazardClasses: HazardClass[] = [];

  sdsForm!: FormGroup;
  safetyDataSheets: SafetyDataSheet[] = [];

  constructor(
    private fb: FormBuilder,
    private productsService: ProductsService,
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      sku: ['', Validators.required],
      productName: ['', Validators.required],
      description: [''],
      productCategoryId: [null, Validators.required],
      unitOfMeasureId: [null, Validators.required],
      packSize: ['', Validators.required],
      basePrice: [0, [Validators.required, Validators.min(0)]],
      currency: ['GBP', Validators.required],
      hazardClassId: [null, Validators.required],
      unNumber: [''],
      storageRequirement: [''],
      requiresSds: [false],
      isRestricted: [false],
      isActive: [true]
    });

    this.sdsForm = this.fb.group({
      fileName: ['', Validators.required],
      filePath: ['', Validators.required],
      version: ['', Validators.required],
      effectiveDate: ['', Validators.required],
      uploadedAt: ['', Validators.required],
      uploadedByUserId: [1, Validators.required]
    });

    const idParam = this.route.snapshot.paramMap.get('id');
    const id = idParam ? Number(idParam) : null;

    this.loadLookups();

    if (id !== null && !Number.isNaN(id)) {
      this.isEditMode = true;
      this.productId = id;
      this.loadProduct(id);
      this.loadSafetyDataSheets(id);
    }
  }

  loadLookups(): void {
    this.productsService.getProductCategories().subscribe({
      next: (data) => {
        this.productCategories = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load categories', err);
        this.cdr.detectChanges();
      }
    });

    this.productsService.getUnitsOfMeasure().subscribe({
      next: (data) => {
        this.unitsOfMeasure = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load units', err);
        this.cdr.detectChanges();
      }
    });

    this.productsService.getHazardClasses().subscribe({
      next: (data) => {
        this.hazardClasses = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load hazard classes', err);
        this.cdr.detectChanges();
      }
    });
  }

  loadProduct(id: number): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.productsService.getById(id).subscribe({
      next: (product: Product) => {
        this.form.patchValue({
          sku: product.sku,
          productName: product.productName,
          description: product.description,
          productCategoryId: product.productCategoryId,
          unitOfMeasureId: product.unitOfMeasureId,
          packSize: product.packSize,
          basePrice: product.basePrice,
          currency: product.currency,
          hazardClassId: product.hazardClassId,
          unNumber: product.unNumber,
          storageRequirement: product.storageRequirement,
          requiresSds: product.requiresSds,
          isRestricted: product.isRestricted,
          isActive: product.isActive
        });

        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load product', err);
        this.errorMessage = 'Failed to load product.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadSafetyDataSheets(productId: number): void {
    this.productsService.getSafetyDataSheets(productId).subscribe({
      next: (data) => {
        this.safetyDataSheets = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load SDS records', err);
        this.errorMessage = 'Failed to load SDS records.';
        this.cdr.detectChanges();
      }
    });
  }

  createSafetyDataSheet(): void {
    if (!this.productId) {
      return;
    }

    if (this.sdsForm.invalid) {
      this.sdsForm.markAllAsTouched();
      return;
    }

    const request: CreateSafetyDataSheetRequest = {
      fileName: this.sdsForm.value.fileName,
      filePath: this.sdsForm.value.filePath,
      version: this.sdsForm.value.version,
      effectiveDate: this.sdsForm.value.effectiveDate,
      uploadedAt: this.sdsForm.value.uploadedAt,
      uploadedByUserId: this.sdsForm.value.uploadedByUserId
    };

    this.productsService.createSafetyDataSheet(this.productId, request).subscribe({
      next: () => {
        this.sdsForm.reset({
          fileName: '',
          filePath: '',
          version: '',
          effectiveDate: '',
          uploadedAt: '',
          uploadedByUserId: 1
        });

        this.loadSafetyDataSheets(this.productId!);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to create SDS', err);
        this.errorMessage = 'Failed to create SDS.';
        this.cdr.detectChanges();
      }
    });
  }

  deleteSafetyDataSheet(sdsId: number): void {
    if (!this.productId) {
      return;
    }

    if (!confirm('Delete this SDS record?')) {
      return;
    }

    this.productsService.deleteSafetyDataSheet(this.productId, sdsId).subscribe({
      next: () => {
        this.loadSafetyDataSheets(this.productId!);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to delete SDS', err);
        this.errorMessage = 'Failed to delete SDS.';
        this.cdr.detectChanges();
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.cdr.detectChanges();

    const formValue = this.form.value;

    if (this.isEditMode && this.productId !== null) {
      this.productsService.update(this.productId, formValue).subscribe({
        next: () => this.router.navigate(['/products']),
        error: (err) => {
          console.error('Failed to update product', err);
          this.errorMessage = 'Failed to update product.';
          this.isLoading = false;
          this.cdr.detectChanges();
        }
      });
    } else {
      this.productsService.create(formValue).subscribe({
        next: () => this.router.navigate(['/products']),
        error: (err) => {
          console.error('Failed to create product', err);
          this.errorMessage = 'Failed to create product.';
          this.isLoading = false;
          this.cdr.detectChanges();
        }
      });
    }
  }
}
