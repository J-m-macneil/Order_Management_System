import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

import { Address, CreateAddressRequest } from '../../models/address.model';
import { CreateCustomerRequest } from '../../models/create-customer.model';
import { Customer } from '../../models/customer.model';
import { UpdateCustomerRequest } from '../../models/update-customer.model';
import { CustomerContact } from '../../models/customer-contact.model';
import { CreateCustomerContactRequest } from '../../models/customer-contact.model';
import { CustomersService } from '../customers.service';

@Component({
  selector: 'app-customer-form',
  standalone: false,
  templateUrl: './customer-form.component.html',
  styleUrls: ['./customer-form.component.css']
})
export class CustomerFormComponent implements OnInit {
  form!: FormGroup;
  addressForm!: FormGroup;

  isEditMode = false;
  customerId: number | null = null;
  isLoading = false;
  errorMessage = '';
  addresses: Address[] = [];

  contactForm!: FormGroup;
  contacts: CustomerContact[] = [];

  constructor(
    private fb: FormBuilder,
    private customersService: CustomersService,
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef
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

    this.addressForm = this.fb.group({
      addressType: ['', Validators.required],
      siteName: ['', Validators.required],
      line1: ['', Validators.required],
      line2: [''],
      city: ['', Validators.required],
      county: [''],
      postcode: ['', Validators.required],
      country: ['United Kingdom', Validators.required],
      contactName: [''],
      contactPhone: [''],
      deliveryInstructions: [''],
      isPrimary: [false]
    });

    this.contactForm = this.fb.group({
      name: ['', Validators.required],
      jobTitle: [''],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
      isPrimary: [false]
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

        this.getCustomerAddresses(id);
        this.getCustomerContacts(id);
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load customer', err);
        this.errorMessage = 'Failed to load customer.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  deleteAddress(addressId: number): void {
    if (!this.customerId) {
      return;
    }

    if (!confirm('Are you sure you want to delete this address?')) {
      return;
    }

    this.customersService.deleteAddress(this.customerId, addressId).subscribe({
      next: () => {
        this.getCustomerAddresses(this.customerId!);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to delete address', err);
        this.errorMessage = 'Failed to delete address.';
        this.cdr.detectChanges();
      }
    });
  }

  getCustomerAddresses(id: number): void {
    this.customersService.getAddresses(id).subscribe({
      next: (data) => {
        console.log('Addresses from API:', data);
        this.addresses = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load addresses', err);
        this.errorMessage = 'Failed to load addresses.';
        this.cdr.detectChanges();
      }
    });
  }

  getCustomerContacts(id: number): void {
    this.customersService.getContacts(id).subscribe({
      next: (data) => {
        this.contacts = data;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load contacts', err);
        this.errorMessage = 'Failed to load contacts.';
        this.cdr.detectChanges();
      }
    });
  }

  createContact(): void {
    if (!this.customerId) {
      return;
    }

    if (this.contactForm.invalid) {
      this.contactForm.markAllAsTouched();
      return;
    }

    const request: CreateCustomerContactRequest = {
      name: this.contactForm.value.name,
      jobTitle: this.contactForm.value.jobTitle,
      email: this.contactForm.value.email,
      phone: this.contactForm.value.phone,
      isPrimary: this.contactForm.value.isPrimary
    };

    this.customersService.createContact(this.customerId, request).subscribe({
      next: () => {
        this.contactForm.reset({
          name: '',
          jobTitle: '',
          email: '',
          phone: '',
          isPrimary: false
        });

        this.getCustomerContacts(this.customerId!);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to create contact', err);
        this.errorMessage = 'Failed to create contact.';
        this.cdr.detectChanges();
      }
    });
  }

  deleteContact(contactId: number): void {
    if (!this.customerId) {
      return;
    }

    if (!confirm('Are you sure you want to delete this contact?')) {
      return;
    }

    this.customersService.deleteContact(this.customerId, contactId).subscribe({
      next: () => {
        this.getCustomerContacts(this.customerId!);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to delete contact', err);
        this.errorMessage = 'Failed to delete contact.';
        this.cdr.detectChanges();
      }
    });
  }

  createAddress(): void {
    if (!this.customerId) {
      return;
    }

    if (this.addressForm.invalid) {
      this.addressForm.markAllAsTouched();
      return;
    }

    const request: CreateAddressRequest = {
      addressType: this.addressForm.value.addressType,
      siteName: this.addressForm.value.siteName,
      line1: this.addressForm.value.line1,
      line2: this.addressForm.value.line2,
      city: this.addressForm.value.city,
      county: this.addressForm.value.county,
      postcode: this.addressForm.value.postcode,
      country: this.addressForm.value.country,
      contactName: this.addressForm.value.contactName,
      contactPhone: this.addressForm.value.contactPhone,
      deliveryInstructions: this.addressForm.value.deliveryInstructions,
      isPrimary: this.addressForm.value.isPrimary
    };

    this.customersService.createAddress(this.customerId, request).subscribe({
      next: () => {
        this.addressForm.reset({
          addressType: '',
          siteName: '',
          line1: '',
          line2: '',
          city: '',
          county: '',
          postcode: '',
          country: 'United Kingdom',
          contactName: '',
          contactPhone: '',
          deliveryInstructions: '',
          isPrimary: false
        });

        this.getCustomerAddresses(this.customerId!);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to create address', err);
        this.errorMessage = 'Failed to create address.';
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
          this.cdr.detectChanges();
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
          this.cdr.detectChanges();
        }
      });
    }
  }
}
