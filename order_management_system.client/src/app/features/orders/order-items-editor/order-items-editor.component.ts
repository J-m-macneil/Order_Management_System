import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormArray, FormGroup } from '@angular/forms';

import { ProductList } from '../../../core/models/product-list.model';

@Component({
  selector: 'app-order-items-editor',
  standalone: false,
  templateUrl: './order-items-editor.component.html'
})
export class OrderItemsEditorComponent {
  @Input({ required: true }) items!: FormArray<FormGroup>;
  @Input() products: ProductList[] = [];

  @Output() itemAdded = new EventEmitter<void>();
  @Output() itemRemoved = new EventEmitter<number>();
  @Output() productChanged = new EventEmitter<number>();
}
