import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { CustomerContact } from '../../../core/models/customer-contact.model';

@Component({
  selector: 'app-customer-contacts-section',
  standalone: false,
  templateUrl: './customer-contacts-section.component.html',
  host: { class: 'block' }
})
export class CustomerContactsSectionComponent {
  @Input({ required: true }) contacts: CustomerContact[] = [];
  @Input({ required: true }) contactForm!: FormGroup;
  @Input() isContactFormOpen = false;
  @Input() editingContactId: number | null = null;

  @Output() addContact = new EventEmitter<void>();
  @Output() editContact = new EventEmitter<CustomerContact>();
  @Output() saveContact = new EventEmitter<void>();
  @Output() cancelContact = new EventEmitter<void>();
  @Output() deleteContact = new EventEmitter<CustomerContact>();
  @Output() setPrimaryContact = new EventEmitter<CustomerContact>();
}
