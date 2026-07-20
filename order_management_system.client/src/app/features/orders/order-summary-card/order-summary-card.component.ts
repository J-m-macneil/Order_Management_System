import { Component, Input } from '@angular/core';
import { Order } from '../../../core/models/order.model';

@Component({
  selector: 'app-order-summary-card',
  standalone: false,
  templateUrl: './order-summary-card.component.html',
  styles: [':host { display: block; }']
})
export class OrderSummaryCardComponent {
  @Input({ required: true }) order!: Order;
}
