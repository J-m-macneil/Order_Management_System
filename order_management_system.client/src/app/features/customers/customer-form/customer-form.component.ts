import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import {
  CustomersService,
  CreateCustomerRequest,
  UpdateCustomerRequest,
  Customer
} from '../customers.service';

@Component({
  selector: 'app-customer-form',
  standalone: false,
  templateUrl: './customer-form.component.html',
  styleUrls: ['./customer-form.component.css']
})
export class CustomerFormComponent implements OnInit {
  form!: FormGroup;
  isEditMode = false;
  customerId: number | null = null;
  isLoading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private customersService: CustomersService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      accountNumber: ['', Validators.required],
      companyName: ['', Validators.required],
      industryType: ['', Validators.required],
      mainContactName: ['', Validators.required],
      mainContactEmail: ['', [Validators.required, Validators.email]],
      mainContactPhone: ['', Validators.required],
      billingAddressId: [null],
      defaultDeliveryAddressId: [null],
      pricingTierId: [1, Validators.required],
      paymentTermsDays: [30, Validators.required],
      creditLimit: [0, [Validators.required, Validators.min(0)]],
      isActive: [true]
    });

    const idParam = this.route.snapshot.paramMap.get('id');
    const id = idParam ? Number(idParam) : null;

    if (id !== null && !Number.isNaN(id)) {
      this.isEditMode = true;
      this.customerId = id;
      this.loadCustomer(id);
    }
  }

  loadCustomer(id: number): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.customersService.getById(id).subscribe({
      next: (customer: Customer) => {
        this.form.patchValue({
          accountNumber: customer.accountNumber,
          companyName: customer.companyName,
          industryType: customer.industryType,
          mainContactName: customer.mainContactName,
          mainContactEmail: customer.mainContactEmail,
          mainContactPhone: customer.mainContactPhone,
          billingAddressId: customer.billingAddressId,
          defaultDeliveryAddressId: customer.defaultDeliveryAddressId,
          pricingTierId: customer.pricingTierId,
          paymentTermsDays: customer.paymentTermsDays,
          creditLimit: customer.creditLimit,
          isActive: customer.isActive
        });

        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load customer', err);
        this.errorMessage = 'Failed to load customer.';
        this.isLoading = false;
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

    const formValue = this.form.value;

    if (this.isEditMode && this.customerId !== null) {
      const updateRequest: UpdateCustomerRequest = {
        accountNumber: formValue.accountNumber,
        companyName: formValue.companyName,
        industryType: formValue.industryType,
        mainContactName: formValue.mainContactName,
        mainContactEmail: formValue.mainContactEmail,
        mainContactPhone: formValue.mainContactPhone,
        billingAddressId: formValue.billingAddressId,
        defaultDeliveryAddressId: formValue.defaultDeliveryAddressId,
        pricingTierId: formValue.pricingTierId,
        paymentTermsDays: formValue.paymentTermsDays,
        creditLimit: formValue.creditLimit,
        isActive: formValue.isActive
      };

      this.customersService.update(this.customerId, updateRequest).subscribe({
        next: () => this.router.navigate(['/customers']),
        error: (err) => {
          console.error('Failed to update customer', err);
          this.errorMessage = 'Failed to update customer.';
          this.isLoading = false;
        }
      });
    } else {
      const createRequest: CreateCustomerRequest = {
        accountNumber: formValue.accountNumber,
        companyName: formValue.companyName,
        industryType: formValue.industryType,
        mainContactName: formValue.mainContactName,
        mainContactEmail: formValue.mainContactEmail,
        mainContactPhone: formValue.mainContactPhone,
        billingAddressId: formValue.billingAddressId,
        defaultDeliveryAddressId: formValue.defaultDeliveryAddressId,
        pricingTierId: formValue.pricingTierId,
        paymentTermsDays: formValue.paymentTermsDays,
        creditLimit: formValue.creditLimit,
        isActive: formValue.isActive
      };

      this.customersService.create(createRequest).subscribe({
        next: () => this.router.navigate(['/customers']),
        error: (err) => {
          console.error('Failed to create customer', err);
          this.errorMessage = 'Failed to create customer.';
          this.isLoading = false;
        }
      });
    }
  }
}
