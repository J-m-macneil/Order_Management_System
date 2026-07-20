import { Component, Input } from '@angular/core';
import { OrderItem } from '../../../core/models/order-item.model';

@Component({
  selector: 'app-order-items-table',
  standalone: false,
  templateUrl: './order-items-table.component.html'
})
export class OrderItemsTableComponent {
  @Input() items: OrderItem[] = [];
  @Input() currency = 'GBP';
}
