import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { OrdersService } from '../../../core/services/orders.service';
import { CustomersService } from '../../../core/services/customers.service';
import { ProductsService } from '../../../core/services/products.service';
import { WarehousesService } from '../../../core/services/warehouses.service';
import { CarriersService } from '../../../core/services/carriers.service';
import { ProjectsService } from '../../../core/services/projects.service';
import { Address } from '../../../core/models/address.model';
import { Customer } from '../../../core/models/customer.model';
import { ProductList } from '../../../core/models/product-list.model';
import { Order } from '../../../core/models/order.model';
import { OrderStatus } from '../../../core/models/order-status.enum';
import { Warehouse } from '../../../core/models/warehouse-model';
import { Carrier } from '../../../core/models/carrier.model';
import { Project } from '../../../core/models/project.model';
import { CreateOrder } from '../../../core/models/create-order.model';
import { ApiErrorResponse, getApiErrorMessage } from '../../../core/utils/api-error-message';

@Component({
  selector: 'app-order-create',
  standalone: false,
  templateUrl: './order-create.component.html'
})
export class OrderCreateComponent implements OnInit {
  orderForm!: FormGroup;
  private readonly customerLookupPageSize = 100;
  private readonly productLookupPageSize = 100;
  orderId: number | null = null;

  isEditMode = false;
  isLoadingOrder = false;
  isSaving = false;
  errorMessage = '';
  orderNumber = '';
  customers: Customer[] = [];
  billingAddresses: Address[] = [];
  deliveryAddresses: Address[] = [];
  products: ProductList[] = [];
  warehouses: Warehouse[] = [];
  carriers: Carrier[] = [];
  projects: Project[] = [];

  constructor(
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router,
    private ordersService: OrdersService,
    private customersService: CustomersService,
    private productsService: ProductsService,
    private warehousesService: WarehousesService,
    private carriersService: CarriersService,
    private projectsService: ProjectsService
  ) { }

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    this.orderId = idParam ? Number(idParam) : null;
    this.isEditMode = !!this.orderId;

    this.orderForm = this.fb.group({
      customerId: [null, Validators.required],
      billingAddressId: [null, Validators.required],
      deliveryAddressId: [null, Validators.required],
      warehouseId: [null, Validators.required],
      carrierId: [null],
      projectId: [null],
      requestedDeliveryDate: [null, Validators.required],
      purchaseOrderReference: ['', [Validators.maxLength(40)]],
      specialInstructions: ['', [Validators.maxLength(255)]],
      internalNotes: ['', [Validators.maxLength(255)]],
      isPriorityOrder: [false],
      items: this.fb.array([this.createItemFormGroup()])
    });

    this.orderForm.get('customerId')?.valueChanges.subscribe((customerId) => {
      this.onCustomerChange(customerId);
    });

    this.loadCustomers();
    this.loadProducts();
    this.loadWarehouses();
    this.loadCarriers();
    this.loadProjects();

    if (this.isEditMode && this.orderId) {
      this.loadOrderForEdit(this.orderId);
    }
  }

  get items(): FormArray {
    return this.orderForm.get('items') as FormArray;
  }

  private createItemFormGroup(): FormGroup {
    return this.fb.group({
      productId: [null, Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]],
      unitPrice: [0, [Validators.required, Validators.min(0)]],
      discountPercent: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
      notes: ['', [Validators.maxLength(255)]]
    });
  }

  private loadCustomers(): void {
    this.customersService.getAll({
      pageNumber: 1,
      pageSize: this.customerLookupPageSize
    }).subscribe({
      next: (data) => {
        this.customers = data.items;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Failed to load customers', err);
      }
    });
  }

  private loadProducts(): void {
    this.productsService.getAll({
      pageNumber: 1,
      pageSize: this.productLookupPageSize
    }).subscribe({
      next: (data) => {
        this.products = data.items;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Failed to load products', err);
      }
    });
  }

  private loadWarehouses(): void {
    this.warehousesService.getAll().subscribe({
      next: (data) => {
        this.warehouses = data;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Failed to load warehouses', err);
      }
    });
  }

  private loadCarriers(): void {
    this.carriersService.getAll().subscribe({
      next: (data) => {
        this.carriers = data;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Failed to load carriers', err);
      }
    });
  }

  private loadProjects(): void {
    this.projectsService.getAll().subscribe({
      next: (data) => {
        this.projects = data;
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Failed to load projects', err);
      }
    });
  }

  addItem(): void {
    this.items.push(this.createItemFormGroup());
  }

  removeItem(index: number): void {
    if (this.items.length > 1) {
      this.items.removeAt(index);
    }
  }

  private onCustomerChange(customerId: number | null): void {
    if (!customerId) {
      this.clearCustomerAddresses();
      this.cdr.markForCheck();
      return;
    }

    this.customersService.getAddresses(customerId).subscribe({
      next: (customerAddresses) => {
        this.setCustomerAddresses(customerAddresses);

        const defaultBilling = this.billingAddresses.find(x => x.isPrimary);
        const defaultDelivery = this.deliveryAddresses.find(x => x.isPrimary);

        this.orderForm.patchValue({
          billingAddressId: defaultBilling?.addressId ?? null,
          deliveryAddressId: defaultDelivery?.addressId ?? null
        });

        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Failed to load customer addresses', err);
        this.clearCustomerAddresses();
        this.cdr.markForCheck();
      }
    });
  }

  private loadOrderForEdit(orderId: number): void {
    this.isLoadingOrder = true;
    this.errorMessage = '';

    this.ordersService.getOrderById(orderId).subscribe({
      next: (order) => {
        if (order.orderStatusId !== OrderStatus.Draft) {
          this.errorMessage = 'Only draft orders can be edited. Return this order to Draft before making changes.';
          this.isLoadingOrder = false;
          this.orderForm.disable();
          this.cdr.markForCheck();
          return;
        }

        this.orderNumber = order.orderNumber;
        this.patchFormForEdit(order);
        this.isLoadingOrder = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.errorMessage = 'Failed to load order for editing.';
        this.isLoadingOrder = false;
        this.cdr.markForCheck();
      }
    });
  }

  private patchFormForEdit(order: Order): void {
    this.orderForm.patchValue({
      customerId: order.customerId,
      billingAddressId: order.billingAddressId,
      deliveryAddressId: order.deliveryAddressId,
      warehouseId: order.warehouseId,
      carrierId: order.carrierId ?? null,
      projectId: order.projectId ?? null,
      requestedDeliveryDate: this.toDateInputValue(order.requestedDeliveryDate),
      purchaseOrderReference: order.purchaseOrderReference ?? '',
      specialInstructions: order.specialInstructions ?? '',
      internalNotes: order.internalNotes ?? '',
      isPriorityOrder: order.isPriorityOrder
    }, { emitEvent: false });

    this.customersService.getAddresses(order.customerId).subscribe({
      next: (customerAddresses) => {
        this.setCustomerAddresses(customerAddresses);

        this.orderForm.patchValue({
          billingAddressId: order.billingAddressId,
          deliveryAddressId: order.deliveryAddressId
        }, { emitEvent: false });

        this.cdr.markForCheck();
      },
      error: () => {
        this.clearCustomerAddresses();
        this.cdr.markForCheck();
      }
    });

    this.items.clear();

    order.items.forEach((item) => {
      const itemGroup = this.createItemFormGroup();
      itemGroup.patchValue({
        productId: item.productId,
        quantity: item.quantity,
        unitPrice: item.unitPrice,
        discountPercent: item.discountPercent,
        notes: item.notes ?? ''
      }, { emitEvent: false });

      this.items.push(itemGroup);
    });

    if (this.items.length === 0) {
      this.items.push(this.createItemFormGroup());
    }
  }

  onProductChange(index: number): void {
    const itemGroup = this.items.at(index) as FormGroup;
    const productId = itemGroup.get('productId')?.value;
    const selectedProduct = this.products.find(x => x.productId === productId);

    itemGroup.patchValue({
      unitPrice: selectedProduct?.basePrice ?? 0,
      discountPercent: 0
    });
  }

  submit(): void {
    if (this.isSaving || this.orderForm.disabled) {
      return;
    }

    if (this.orderForm.invalid) {
      this.orderForm.markAllAsTouched();
      return;
    }

    const dto = this.orderForm.getRawValue() as CreateOrder;
    this.isSaving = true;
    this.errorMessage = '';

    if (this.isEditMode && this.orderId) {
      this.ordersService.updateOrder(this.orderId, dto).subscribe({
        next: () => {
          this.router.navigate(['/orders', this.orderId]);
        },
        error: (err) => {
          this.onSaveError(err, 'Failed to update order.');
        }
      });

      return;
    }

    this.ordersService.createOrder(dto).subscribe({
      next: (result) => {
        this.router.navigate(['/orders', result.orderId]);
      },
      error: (err) => {
        this.onSaveError(err, 'Failed to create order.');
      }
    });
  }

  private setCustomerAddresses(addresses: Address[]): void {
    this.billingAddresses = addresses.filter(address => address.addressType === 'Billing');
    this.deliveryAddresses = addresses.filter(address => address.addressType === 'DeliverySite');
  }

  private clearCustomerAddresses(): void {
    this.billingAddresses = [];
    this.deliveryAddresses = [];
    this.orderForm.patchValue({
      billingAddressId: null,
      deliveryAddressId: null
    });
  }

  private onSaveError(error: ApiErrorResponse, fallbackMessage: string): void {
    this.errorMessage = getApiErrorMessage(error, fallbackMessage);
    this.isSaving = false;
    this.cdr.markForCheck();
  }

  private toDateInputValue(value: string): string {
    return value ? value.substring(0, 10) : '';
  }
}
