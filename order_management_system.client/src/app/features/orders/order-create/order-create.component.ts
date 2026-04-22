import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { OrdersService } from '../../../core/services/orders.service';
import { CustomersService } from '../../../core/services/customers.service';
import { ProductsService } from '../../../core/services/products.service';
import { WarehousesService } from '../../../core/services/warehouses.service';
import { CarriersService } from '../../../core/services/carriers.service';
import { ProjectsService } from '../../../core/services/projects.service';
import { Address } from '../../../core/models/address.model';
import { Customer } from '../../../core/models/customer.model';
import { ProductList } from '../../../core/models/product-list.model';

@Component({
  selector: 'app-order-create',
  standalone: false,
  templateUrl: './order-create.component.html',
  styleUrls: ['./order-create.component.css']
})
export class OrderCreateComponent implements OnInit {
  orderForm!: FormGroup;

  customers: Customer[] = [];
  billingAddresses: Address[] = [];
  deliveryAddresses: Address[] = [];
  products: ProductList[] = [];
  warehouses: any[] = [];
  carriers: any[] = [];
  projects: any[] = [];

  constructor(
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef,
    private ordersService: OrdersService,
    private customersService: CustomersService,
    private productsService: ProductsService,
    private warehousesService: WarehousesService,
    private carriersService: CarriersService,
    private projectsService: ProjectsService
  ) { }

  ngOnInit(): void {
    this.orderForm = this.fb.group({
      customerId: [null, Validators.required],
      billingAddressId: [null, Validators.required],
      deliveryAddressId: [null, Validators.required],
      warehouseId: [null, Validators.required],
      carrierId: [null],
      projectId: [null],
      createdByUserId: [1, Validators.required],
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
    this.cdr.detectChanges();
  }

  get items(): FormArray {
    return this.orderForm.get('items') as FormArray;
  }

  createItemFormGroup(): FormGroup {
    const group = this.fb.group({
      productId: [null, Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]],
      unitPrice: [0, [Validators.required, Validators.min(0)]],
      discountPercent: [0, [Validators.required, Validators.min(0)]],
      notes: ['', [Validators.maxLength(255)]]
    });

    group.get('productId')?.valueChanges.subscribe(() => {
      const index = this.items.controls.indexOf(group);
      if (index !== -1) {
        this.onProductChange(index);
      }
    });

    return group;
  }

  loadCustomers(): void {
    this.customersService.getAll().subscribe({
      next: (data) => {
        this.customers = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load customers', err);
        this.cdr.detectChanges();
      }
    });
  }

  loadProducts(): void {
    this.productsService.getAll().subscribe({
      next: (data) => {
        this.products = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load products', err);
        this.cdr.detectChanges();
      }
    });
  }

  loadWarehouses(): void {
    this.warehousesService.getAll().subscribe({
      next: (data) => {
        this.warehouses = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load warehouses', err);
        this.cdr.detectChanges();
      }
    });
  }

  loadCarriers(): void {
    this.carriersService.getAll().subscribe({
      next: (data) => {
        this.carriers = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load carriers', err);
        this.cdr.detectChanges();
      }
    });
  }

  loadProjects(): void {
    this.projectsService.getAll().subscribe({
      next: (data) => {
        this.projects = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load projects', err);
        this.cdr.detectChanges();
      }
    });
  }

  addItem(): void {
    this.items.push(this.createItemFormGroup());
    this.cdr.detectChanges();
  }

  removeItem(index: number): void {
    if (this.items.length > 1) {
      this.items.removeAt(index);
      this.cdr.detectChanges();
    }
  }

  onCustomerChange(customerId: number | null): void {
    if (!customerId) {
      this.billingAddresses = [];
      this.deliveryAddresses = [];
      this.orderForm.patchValue({
        billingAddressId: null,
        deliveryAddressId: null
      });
      this.cdr.detectChanges();
      return;
    }

    this.customersService.getAddresses(customerId).subscribe({
      next: (customerAddresses) => {
        this.billingAddresses = customerAddresses.filter(
          x => x.addressType === 'Billing'
        );

        this.deliveryAddresses = customerAddresses.filter(
          x => x.addressType === 'DeliverySite'
        );

        const defaultBilling = this.billingAddresses.find(x => x.isPrimary);
        const defaultDelivery = this.deliveryAddresses.find(x => x.isPrimary);

        this.orderForm.patchValue({
          billingAddressId: defaultBilling?.addressId ?? null,
          deliveryAddressId: defaultDelivery?.addressId ?? null
        });

        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load customer addresses', err);
        this.billingAddresses = [];
        this.deliveryAddresses = [];
        this.orderForm.patchValue({
          billingAddressId: null,
          deliveryAddressId: null
        });
        this.cdr.detectChanges();
      }
    });
  }

  onProductChange(index: number): void {
    const itemGroup = this.items.at(index) as FormGroup;
    const productId = itemGroup.get('productId')?.value;

    if (!productId) {
      itemGroup.patchValue({
        unitPrice: 0,
        discountPercent: 0
      });
      this.cdr.detectChanges();
      return;
    }

    const selectedProduct = this.products.find(x => x.productId === productId);

    if (!selectedProduct) {
      itemGroup.patchValue({
        unitPrice: 0,
        discountPercent: 0
      });
      this.cdr.detectChanges();
      return;
    }

    itemGroup.patchValue({
      unitPrice: selectedProduct.basePrice,
      discountPercent: 0
    });

    this.cdr.detectChanges();
  }

  submit(): void {
    if (this.orderForm.invalid) {
      this.orderForm.markAllAsTouched();
      this.cdr.detectChanges();
      return;
    }

    const dto = this.orderForm.value;
    console.log(dto);

    this.ordersService.createOrder(dto).subscribe({
      next: (result) => {
        console.log('Order created', result);
        this.orderForm.reset({
          customerId: null,
          billingAddressId: null,
          deliveryAddressId: null,
          warehouseId: null,
          carrierId: null,
          projectId: null,
          createdByUserId: 1,
          requestedDeliveryDate: null,
          purchaseOrderReference: '',
          specialInstructions: '',
          internalNotes: '',
          isPriorityOrder: false
        });
        this.items.clear();
        this.items.push(this.createItemFormGroup());
        this.billingAddresses = [];
        this.deliveryAddresses = [];
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to create order', err);
        this.cdr.detectChanges();
      }
    });
  }
}
