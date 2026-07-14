import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HazardClass } from '../../../core/models/hazard-class.model';
import { ProductCategory } from '../../../core/models/product-category.model';
import { Product } from '../../../core/models/product.model';
import { UnitOfMeasure } from '../../../core/models/unit-of-measure.model';
import { ProductsService } from '../../../core/services/products.service';
import { ProductAuditPanelComponent } from '../product-audit-panel/product-audit-panel.component';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  standalone: false
})
export class ProductFormComponent implements OnInit {
  @ViewChild(ProductAuditPanelComponent) private auditPanel?: ProductAuditPanelComponent;

  form!: FormGroup;

  isEditMode = false;
  productId: number | null = null;
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  savedRequiresSds = false;

  productCategories: ProductCategory[] = [];
  unitsOfMeasure: UnitOfMeasure[] = [];
  hazardClasses: HazardClass[] = [];

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

    const idParam = this.route.snapshot.paramMap.get('id');
    const id = idParam ? Number(idParam) : null;

    this.loadLookups();

    if (id !== null && !Number.isNaN(id)) {
      this.isEditMode = true;
      this.productId = id;
      this.loadProduct(id);
    }
  }

  private loadLookups(): void {
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

  private loadProduct(id: number): void {
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

        this.savedRequiresSds = product.requiresSds;
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

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';
    this.cdr.detectChanges();

    const formValue = this.form.value;

    if (this.isEditMode && this.productId !== null) {
      this.productsService.update(this.productId, formValue).subscribe({
        next: () => {
          this.successMessage = 'Product saved.';
          this.savedRequiresSds = Boolean(this.form.get('requiresSds')?.value);
          this.isLoading = false;
          this.auditPanel?.reload();
          this.cdr.detectChanges();
        },
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

  onSdsChanged(): void {
    this.auditPanel?.reload();
  }
}
