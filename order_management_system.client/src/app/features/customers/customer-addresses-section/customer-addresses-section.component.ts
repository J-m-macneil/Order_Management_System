import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Address } from '../../../core/models/address.model';

@Component({
  selector: 'app-customer-addresses-section',
  standalone: false,
  templateUrl: './customer-addresses-section.component.html',
  host: { class: 'block' }
})
export class CustomerAddressesSectionComponent {
  @Input({ required: true }) addresses: Address[] = [];
  @Input({ required: true }) addressForm!: FormGroup;
  @Input() isAddressFormOpen = false;
  @Input() editingAddressId: number | null = null;
  @Input() billingAddressId: number | null = null;
  @Input() defaultDeliveryAddressId: number | null = null;
  @Input() isLoading = false;

  @Output() addAddress = new EventEmitter<void>();
  @Output() editAddress = new EventEmitter<Address>();
  @Output() saveAddress = new EventEmitter<void>();
  @Output() cancelAddress = new EventEmitter<void>();
  @Output() deleteAddress = new EventEmitter<Address>();
  @Output() setBillingAddress = new EventEmitter<Address>();
  @Output() setDefaultDeliveryAddress = new EventEmitter<Address>();
}
