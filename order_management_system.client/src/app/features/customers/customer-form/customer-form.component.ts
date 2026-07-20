import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Address, CreateAddressRequest } from '../../../core/models/address.model';
import { CreateCustomerRequest } from '../../../core/models/create-customer.model';
import { CustomerContact, CreateCustomerContactRequest, UpdateCustomerContactRequest } from '../../../core/models/customer-contact.model';
import { Customer } from '../../../core/models/customer.model';
import { UpdateCustomerRequest } from '../../../core/models/update-customer.model';
import { CustomersService } from '../../../core/services/customers.service';
import { PricingTier } from '../../../core/models/pricing-tier.model';
import { PricingService } from '../../../core/services/pricing.service';
import { ToastService } from '../../../core/services/toast.service';
import { ApiErrorResponse, getApiErrorMessage } from '../../../core/utils/api-error-message';
import { getValidationMessage, PHONE_NUMBER_PATTERN } from '../../../core/utils/form-validation';
import { forkJoin, map, switchMap } from 'rxjs';

@Component({
  selector: 'app-customer-form',
  standalone: false,
  templateUrl: './customer-form.component.html'
})
export class CustomerFormComponent implements OnInit {
  readonly validationMessage = getValidationMessage;

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
  isContactFormOpen = false;
  editingContactId: number | null = null;
  isAddressFormOpen = false;
  editingAddressId: number | null = null;
  addressPendingDelete: Address | null = null;
  contactPendingDelete: CustomerContact | null = null;
  pricingTiers: PricingTier[] = [];

  constructor(
    private fb: FormBuilder,
    private customersService: CustomersService,
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private pricingService: PricingService,
    private toastService: ToastService
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      accountNumber: ['', Validators.required],
      companyName: ['', Validators.required],
      industryType: ['', Validators.required],
      mainContactName: [''],
      mainContactEmail: [''],
      mainContactPhone: [''],
      billingAddressId: [null],
      defaultDeliveryAddressId: [null],
      pricingTierId: [1, Validators.required],
      paymentTermsDays: [30, [Validators.required, Validators.min(0)]],
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
      contactPhone: ['', Validators.pattern(PHONE_NUMBER_PATTERN)],
      deliveryInstructions: [''],
      isPrimary: [false]
    });

    this.contactForm = this.fb.group({
      name: ['', Validators.required],
      jobTitle: [''],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', Validators.pattern(PHONE_NUMBER_PATTERN)],
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
        this.cdr.markForCheck();
      },
      error: (err) => {
        console.error('Failed to load customer', err);
        this.errorMessage = 'Failed to load customer.';
        this.isLoading = false;
        this.cdr.markForCheck();
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

        this.cdr.markForCheck();
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
      contactPhone: ['', Validators.pattern(PHONE_NUMBER_PATTERN)],
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

  private buildCurrentCustomerUpdateRequest(): UpdateCustomerRequest {
    return this.buildCustomerRequest(
      this.form.value,
      this.form.value.billingAddressId,
      this.form.value.defaultDeliveryAddressId
    ) as UpdateCustomerRequest;
  }

  private buildContactRequest(isPrimary = false): CreateCustomerContactRequest {
    const value = this.contactForm.value;

    return {
      name: value.name,
      jobTitle: value.jobTitle,
      email: value.email,
      phone: value.phone,
      isPrimary: isPrimary || value.isPrimary
    };
  }

  private applyPrimaryContactToCustomerForm(): void {
    const value = this.contactForm.value;

    this.form.patchValue({
      mainContactName: value.name,
      mainContactEmail: value.email,
      mainContactPhone: value.phone
    });
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

  saveCustomerDetails(): void {
    if (!this.customerId) {
      return;
    }

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.customersService.update(this.customerId, this.buildCurrentCustomerUpdateRequest()).subscribe({
      next: () => {
        this.toastService.success('Customer updated', 'The customer details were saved.');
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (err: ApiErrorResponse) => {
        console.error('Failed to update customer', err);
        this.toastService.error('Customer update failed', getApiErrorMessage(err, 'The customer details could not be saved.'));
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  openDeleteAddressModal(address: Address): void {
    this.addressPendingDelete = address;
  }

  cancelDeleteAddress(): void {
    this.addressPendingDelete = null;
  }

  confirmDeleteAddress(): void {
    if (!this.customerId) {
      return;
    }

    if (!this.addressPendingDelete) {
      return;
    }

    this.customersService.deleteAddress(this.customerId, this.addressPendingDelete.addressId).subscribe({
      next: () => {
        this.addressPendingDelete = null;
        this.toastService.success('Address deleted', 'The address was removed from the customer.');
        this.getCustomerAddresses(this.customerId!);
        this.cdr.markForCheck();
      },
      error: (err: ApiErrorResponse) => {
        console.error('Failed to delete address', err);
        this.toastService.error('Address deletion failed', getApiErrorMessage(err, 'The address could not be deleted.'));
        this.addressPendingDelete = null;
        this.cdr.markForCheck();
      }
    });
  }

  getCustomerAddresses(id: number, customer?: Customer): void {
    this.customersService.getAddresses(id).subscribe({
      next: (data) => {
        this.addresses = data;

        if (customer) {
          this.populateAddressForms(customer, data);
        }

        this.cdr.markForCheck();
      },
      error: (err: ApiErrorResponse) => {
        console.error('Failed to load addresses', err);
        this.toastService.error('Addresses unavailable', getApiErrorMessage(err, 'Customer addresses could not be loaded.'));
        this.cdr.markForCheck();
      }
    });
  }

  getCustomerContacts(id: number): void {
    this.customersService.getContacts(id).subscribe({
      next: (data) => {
        this.contacts = data;
        this.cdr.markForCheck();
      },
      error: (err: ApiErrorResponse) => {
        console.error('Failed to load contacts', err);
        this.toastService.error('Contacts unavailable', getApiErrorMessage(err, 'Customer contacts could not be loaded.'));
        this.cdr.markForCheck();
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

    const request = this.buildContactRequest();

    if (this.editingContactId) {
      this.updateContact(request);
      return;
    }

    this.customersService.createContact(this.customerId, request).subscribe({
      next: () => {
        this.resetContactForm();
        this.toastService.success('Contact added', 'The contact was added to the customer.');
        this.getCustomerContacts(this.customerId!);
        this.cdr.markForCheck();
      },
      error: (err: ApiErrorResponse) => {
        console.error('Failed to create contact', err);
        this.toastService.error('Contact creation failed', getApiErrorMessage(err, 'The contact could not be added.'));
        this.cdr.markForCheck();
      }
    });
  }

  openContactForm(): void {
    this.isContactFormOpen = true;
    this.resetContactForm(false);
  }

  setBillingAddress(address: Address): void {
    if (!this.customerId) {
      return;
    }

    const defaultDeliveryAddressId = this.form.value.deliverySameAsBilling
      ? address.addressId
      : this.form.value.defaultDeliveryAddressId;

    this.updateCustomerAddressReferences(
      address.addressId,
      defaultDeliveryAddressId,
      'Billing address selected.'
    );
  }

  setDefaultDeliveryAddress(address: Address): void {
    if (!this.customerId) {
      return;
    }

    this.updateCustomerAddressReferences(
      this.form.value.billingAddressId,
      address.addressId,
      'Default delivery address selected.'
    );
  }

  private updateCustomerAddressReferences(
    billingAddressId: number | null,
    defaultDeliveryAddressId: number | null,
    successMessage: string
  ): void {
    if (!this.customerId) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const updateRequest = this.buildCustomerRequest(
      this.form.value,
      billingAddressId,
      defaultDeliveryAddressId
    ) as UpdateCustomerRequest;

    this.customersService.update(this.customerId, updateRequest).subscribe({
      next: () => {
        this.form.patchValue({
          billingAddressId,
          defaultDeliveryAddressId,
          deliverySameAsBilling: !!billingAddressId && billingAddressId === defaultDeliveryAddressId
        });
        this.toastService.success('Address preference updated', successMessage);
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (err: ApiErrorResponse) => {
        console.error('Failed to update customer address references', err);
        this.toastService.error('Address preference update failed', getApiErrorMessage(err, 'The address selection could not be saved.'));
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  editContact(contact: CustomerContact): void {
    this.isContactFormOpen = true;
    this.editingContactId = contact.customerContactId;

    this.contactForm.patchValue({
      name: contact.name,
      jobTitle: contact.jobTitle ?? '',
      email: contact.email,
      phone: contact.phone ?? '',
      isPrimary: contact.isPrimary
    });
  }

  setPrimaryContact(contact: CustomerContact): void {
    if (!this.customerId || contact.isPrimary) {
      return;
    }

    const request: UpdateCustomerContactRequest = {
      name: contact.name,
      jobTitle: contact.jobTitle,
      email: contact.email,
      phone: contact.phone,
      isPrimary: true
    };

    this.customersService.updateContact(this.customerId, contact.customerContactId, request).subscribe({
      next: () => {
        this.resetContactForm();
        this.toastService.success('Primary contact updated', `${contact.name} is now the primary contact.`);
        this.getCustomerContacts(this.customerId!);
        this.cdr.markForCheck();
      },
      error: (err: ApiErrorResponse) => {
        console.error('Failed to set primary contact', err);
        this.toastService.error('Primary contact update failed', getApiErrorMessage(err, 'The primary contact could not be changed.'));
        this.cdr.markForCheck();
      }
    });
  }

  cancelContactEdit(): void {
    this.resetContactForm();
  }

  private updateContact(request: UpdateCustomerContactRequest): void {
    if (!this.customerId || !this.editingContactId) {
      return;
    }

    this.customersService.updateContact(this.customerId, this.editingContactId, request).subscribe({
      next: () => {
        this.resetContactForm();
        this.toastService.success('Contact updated', 'The contact details were saved.');
        this.getCustomerContacts(this.customerId!);
        this.cdr.markForCheck();
      },
      error: (err: ApiErrorResponse) => {
        console.error('Failed to update contact', err);
        this.toastService.error('Contact update failed', getApiErrorMessage(err, 'The contact details could not be saved.'));
        this.cdr.markForCheck();
      }
    });
  }

  private resetContactForm(closeForm = true): void {
    this.editingContactId = null;

    if (closeForm) {
      this.isContactFormOpen = false;
    }

    this.contactForm.reset({
      name: '',
      jobTitle: '',
      email: '',
      phone: '',
      isPrimary: false
    });
  }

  openDeleteContactModal(contact: CustomerContact): void {
    this.contactPendingDelete = contact;
  }

  cancelDeleteContact(): void {
    this.contactPendingDelete = null;
  }

  confirmDeleteContact(): void {
    if (!this.customerId) {
      return;
    }

    if (!this.contactPendingDelete) {
      return;
    }

    this.customersService.deleteContact(this.customerId, this.contactPendingDelete.customerContactId).subscribe({
      next: () => {
        this.contactPendingDelete = null;
        this.toastService.success('Contact deleted', 'The contact was removed from the customer.');
        this.getCustomerContacts(this.customerId!);
        this.cdr.markForCheck();
      },
      error: (err: ApiErrorResponse) => {
        console.error('Failed to delete contact', err);
        this.toastService.error('Contact deletion failed', getApiErrorMessage(err, 'The contact could not be deleted.'));
        this.contactPendingDelete = null;
        this.cdr.markForCheck();
      }
    });
  }

  openAddressForm(addressType: 'Billing' | 'DeliverySite' | 'WarehousePartner' = 'DeliverySite'): void {
    this.isAddressFormOpen = true;
    this.editingAddressId = null;

    this.addressForm.reset({
      addressType,
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
  }

  editAddress(address: Address): void {
    this.isAddressFormOpen = true;
    this.editingAddressId = address.addressId;
    this.patchAddressForm(this.addressForm, address, address.addressType === 'Billing' ? 'Billing' : 'DeliverySite');
  }

  cancelAddressEdit(): void {
    this.isAddressFormOpen = false;
    this.editingAddressId = null;

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
  }

  saveAddressRecord(): void {
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

    const save$ = this.editingAddressId
      ? this.customersService.updateAddress(this.customerId, this.editingAddressId, request).pipe(
          map(() => ({ addressId: this.editingAddressId! }))
        )
      : this.customersService.createAddress(this.customerId, request);

    save$.subscribe({
      next: () => {
        const wasEditing = this.editingAddressId !== null;
        this.toastService.success(
          wasEditing ? 'Address updated' : 'Address added',
          wasEditing ? 'The address details were saved.' : 'The address was added to the customer.'
        );
        this.cancelAddressEdit();
        this.getCustomerAddresses(this.customerId!);
        this.cdr.markForCheck();
      },
      error: (err: ApiErrorResponse) => {
        console.error('Failed to save address', err);
        this.toastService.error('Address save failed', getApiErrorMessage(err, 'The address could not be saved.'));
        this.cdr.markForCheck();
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (!this.isEditMode && this.contactForm.invalid) {
      this.contactForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const formValue = this.form.value;

    if (this.isEditMode && this.customerId !== null) {
      this.saveCustomerDetails();
    } else {
      const deliverySameAsBilling = formValue.deliverySameAsBilling;

      if (this.billingAddressForm.invalid || (!deliverySameAsBilling && this.deliveryAddressForm.invalid)) {
        this.billingAddressForm.markAllAsTouched();

        if (!deliverySameAsBilling) {
          this.deliveryAddressForm.markAllAsTouched();
        }

        this.isLoading = false;
        this.cdr.markForCheck();
        return;
      }

      this.applyPrimaryContactToCustomerForm();
      const primaryContactRequest = this.buildContactRequest(true);
      const createFormValue = this.form.value;
      const createRequest = this.buildCustomerRequest(createFormValue, null, null);

      this.customersService.create(createRequest).pipe(
        switchMap((customer) => {
          const billingRequest = this.buildAddressRequest(this.billingAddressForm, 'Billing');

          if (deliverySameAsBilling) {
            return this.customersService.createAddress(customer.customerId, billingRequest).pipe(
              switchMap((billingAddress) => {
                const updateRequest = this.buildCustomerRequest(
                  createFormValue,
                  billingAddress.addressId,
                  billingAddress.addressId
                ) as UpdateCustomerRequest;

                return forkJoin({
                  contact: this.customersService.createContact(customer.customerId, primaryContactRequest),
                  customer: this.customersService.update(customer.customerId, updateRequest)
                });
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
                createFormValue,
                billingAddress.addressId,
                deliveryAddress.addressId
              ) as UpdateCustomerRequest;

              return forkJoin({
                contact: this.customersService.createContact(customer.customerId, primaryContactRequest),
                customer: this.customersService.update(customer.customerId, updateRequest)
              });
            })
          );
        })
      ).subscribe({
        next: () => {
          this.toastService.success('Customer created', 'The customer account was created successfully.');
          this.router.navigate(['/customers']);
        },
        error: (err: ApiErrorResponse) => {
          console.error('Failed to create customer', err);
          this.toastService.error('Customer creation failed', getApiErrorMessage(err, 'The customer account could not be created.'));
          this.isLoading = false;
          this.cdr.markForCheck();
        }
      });
    }
  }
}
