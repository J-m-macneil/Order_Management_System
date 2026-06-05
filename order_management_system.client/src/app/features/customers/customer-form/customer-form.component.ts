import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Address, CreateAddressRequest } from '../../../core/models/address.model';
import { CreateCustomerRequest } from '../../../core/models/create-customer.model';
import { CustomerContact, CreateCustomerContactRequest } from '../../../core/models/customer-contact.model';
import { Customer } from '../../../core/models/customer.model';
import { UpdateCustomerRequest } from '../../../core/models/update-customer.model';
import { CustomersService } from '../../../core/services/customers.service';
import { PricingTier } from '../../../core/models/pricing-tier.model';
import { PricingService } from '../../../core/services/pricing.service';
import { forkJoin, map, Observable, switchMap } from 'rxjs';

@Component({
  selector: 'app-customer-form',
  standalone: false,
  templateUrl: './customer-form.component.html',
  styleUrls: ['./customer-form.component.css']
})
export class CustomerFormComponent implements OnInit {
  form!: FormGroup;
  addressForm!: FormGroup;
  billingAddressForm!: FormGroup;
  deliveryAddressForm!: FormGroup;

  isEditMode = false;
  customerId: number | null = null;
  isLoading = false;
  errorMessage = '';
  addresses: Address[] = [];

  contactForm!: FormGroup;
  contacts: CustomerContact[] = [];
  pricingTiers: PricingTier[] = [];

  constructor(
    private fb: FormBuilder,
    private customersService: CustomersService,
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private pricingService: PricingService
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
      deliverySameAsBilling: [true],
      isActive: [true]
    });

    this.loadPricingTiers();

    this.billingAddressForm = this.createCustomerAddressForm('Billing', true);
    this.deliveryAddressForm = this.createCustomerAddressForm('DeliverySite', true);

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
          deliverySameAsBilling: !!customer.billingAddressId &&
            customer.billingAddressId === customer.defaultDeliveryAddressId,
          isActive: customer.isActive
        });

        this.getCustomerAddresses(id, customer);
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

  loadPricingTiers(): void {
    this.pricingService.getPricingTiers().subscribe({
      next: (data: PricingTier[]) => {
        this.pricingTiers = data; 

        // If no value set, default to first tier
        if (!this.form.value.pricingTierId && data.length > 0) {
          this.form.patchValue({
            pricingTierId: data[0].pricingTierId
          });
        }

        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load pricing tiers', err);
      }
    });
  }

  createCustomerAddressForm(addressType: 'Billing' | 'DeliverySite', isPrimary: boolean): FormGroup {
    return this.fb.group({
      addressType: [addressType, Validators.required],
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
      isPrimary: [isPrimary]
    });
  }

  private buildAddressRequest(source: FormGroup, addressType: 'Billing' | 'DeliverySite'): CreateAddressRequest {
    const value = source.value;

    return {
      addressType,
      siteName: value.siteName,
      line1: value.line1,
      line2: value.line2,
      city: value.city,
      county: value.county,
      postcode: value.postcode,
      country: value.country,
      contactName: value.contactName,
      contactPhone: value.contactPhone,
      deliveryInstructions: value.deliveryInstructions,
      isPrimary: value.isPrimary
    };
  }

  private buildCustomerRequest(formValue: any, billingAddressId: number | null, defaultDeliveryAddressId: number | null): CreateCustomerRequest {
    return {
      accountNumber: formValue.accountNumber,
      companyName: formValue.companyName,
      industryType: formValue.industryType,
      mainContactName: formValue.mainContactName,
      mainContactEmail: formValue.mainContactEmail,
      mainContactPhone: formValue.mainContactPhone,
      billingAddressId,
      defaultDeliveryAddressId,
      pricingTierId: formValue.pricingTierId,
      paymentTermsDays: formValue.paymentTermsDays,
      creditLimit: formValue.creditLimit,
      isActive: formValue.isActive
    };
  }

  private patchAddressForm(target: FormGroup, address: Address | null, fallbackType: 'Billing' | 'DeliverySite'): void {
    target.patchValue({
      addressType: address?.addressType ?? fallbackType,
      siteName: address?.siteName ?? '',
      line1: address?.line1 ?? '',
      line2: address?.line2 ?? '',
      city: address?.city ?? '',
      county: address?.county ?? '',
      postcode: address?.postcode ?? '',
      country: address?.country ?? 'United Kingdom',
      contactName: address?.contactName ?? '',
      contactPhone: address?.contactPhone ?? '',
      deliveryInstructions: address?.deliveryInstructions ?? '',
      isPrimary: address?.isPrimary ?? true
    });
  }

  private populateAddressForms(customer: Customer, addresses: Address[]): void {
    const billingAddress = addresses.find(x => x.addressId === customer.billingAddressId)
      ?? addresses.find(x => x.addressType === 'Billing' && x.isPrimary)
      ?? addresses.find(x => x.addressType === 'Billing')
      ?? null;

    const deliveryAddress = addresses.find(x => x.addressId === customer.defaultDeliveryAddressId)
      ?? addresses.find(x => x.addressType === 'DeliverySite' && x.isPrimary)
      ?? addresses.find(x => x.addressType === 'DeliverySite')
      ?? null;

    this.patchAddressForm(this.billingAddressForm, billingAddress, 'Billing');
    this.patchAddressForm(this.deliveryAddressForm, deliveryAddress, 'DeliverySite');

    const billingAddressId = billingAddress?.addressId ?? customer.billingAddressId;
    const deliveryAddressId = deliveryAddress?.addressId ?? customer.defaultDeliveryAddressId;

    this.form.patchValue({
      billingAddressId,
      defaultDeliveryAddressId: deliveryAddressId,
      deliverySameAsBilling: !!billingAddressId && billingAddressId === deliveryAddressId
    });
  }

  private saveAddress(
    customerId: number,
    source: FormGroup,
    addressType: 'Billing' | 'DeliverySite',
    addressId: number | null
  ): Observable<number> {
    const request = this.buildAddressRequest(source, addressType);

    if (addressId) {
      return this.customersService.updateAddress(customerId, addressId, request).pipe(
        map(() => addressId)
      );
    }

    return this.customersService.createAddress(customerId, request).pipe(
      map(address => address.addressId)
    );
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

  getCustomerAddresses(id: number, customer?: Customer): void {
    this.customersService.getAddresses(id).subscribe({
      next: (data) => {
        console.log('Addresses from API:', data);
        this.addresses = data;

        if (customer) {
          this.populateAddressForms(customer, data);
        }

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
      const deliverySameAsBilling = formValue.deliverySameAsBilling;

      if (this.billingAddressForm.invalid || (!deliverySameAsBilling && this.deliveryAddressForm.invalid)) {
        this.billingAddressForm.markAllAsTouched();

        if (!deliverySameAsBilling) {
          this.deliveryAddressForm.markAllAsTouched();
        }

        this.isLoading = false;
        this.cdr.detectChanges();
        return;
      }

      const billingAddressId = formValue.billingAddressId;
      const deliveryAddressId = formValue.defaultDeliveryAddressId;

      this.saveAddress(this.customerId, this.billingAddressForm, 'Billing', billingAddressId).pipe(
        switchMap((savedBillingAddressId) => {
          if (deliverySameAsBilling) {
            const updateRequest = this.buildCustomerRequest(
              formValue,
              savedBillingAddressId,
              savedBillingAddressId
            ) as UpdateCustomerRequest;

            return this.customersService.update(this.customerId!, updateRequest);
          }

          return this.saveAddress(
            this.customerId!,
            this.deliveryAddressForm,
            'DeliverySite',
            deliveryAddressId === savedBillingAddressId ? null : deliveryAddressId
          ).pipe(
            switchMap((savedDeliveryAddressId) => {
              const updateRequest = this.buildCustomerRequest(
                formValue,
                savedBillingAddressId,
                savedDeliveryAddressId
              ) as UpdateCustomerRequest;

              return this.customersService.update(this.customerId!, updateRequest);
            })
          );
        })
      ).subscribe({
        next: () => this.router.navigate(['/customers']),
        error: (err) => {
          console.error('Failed to update customer', err);
          this.errorMessage = 'Failed to update customer.';
          this.isLoading = false;
          this.cdr.detectChanges();
        }
      });
    } else {
      const deliverySameAsBilling = formValue.deliverySameAsBilling;

      if (this.billingAddressForm.invalid || (!deliverySameAsBilling && this.deliveryAddressForm.invalid)) {
        this.billingAddressForm.markAllAsTouched();

        if (!deliverySameAsBilling) {
          this.deliveryAddressForm.markAllAsTouched();
        }

        this.isLoading = false;
        this.cdr.detectChanges();
        return;
      }

      const createRequest = this.buildCustomerRequest(formValue, null, null);

      this.customersService.create(createRequest).pipe(
        switchMap((customer) => {
          const billingRequest = this.buildAddressRequest(this.billingAddressForm, 'Billing');

          if (deliverySameAsBilling) {
            return this.customersService.createAddress(customer.customerId, billingRequest).pipe(
              switchMap((billingAddress) => {
                const updateRequest = this.buildCustomerRequest(
                  formValue,
                  billingAddress.addressId,
                  billingAddress.addressId
                ) as UpdateCustomerRequest;

                return this.customersService.update(customer.customerId, updateRequest);
              })
            );
          }

          const deliveryRequest = this.buildAddressRequest(this.deliveryAddressForm, 'DeliverySite');

          return forkJoin({
            billingAddress: this.customersService.createAddress(customer.customerId, billingRequest),
            deliveryAddress: this.customersService.createAddress(customer.customerId, deliveryRequest)
          }).pipe(
            switchMap(({ billingAddress, deliveryAddress }) => {
              const updateRequest = this.buildCustomerRequest(
                formValue,
                billingAddress.addressId,
                deliveryAddress.addressId
              ) as UpdateCustomerRequest;

              return this.customersService.update(customer.customerId, updateRequest);
            })
          );
        })
      ).subscribe({
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
