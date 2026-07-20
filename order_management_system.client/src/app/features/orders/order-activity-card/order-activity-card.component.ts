import { Component, Input } from '@angular/core';
import { Order } from '../../../core/models/order.model';

@Component({
  selector: 'app-order-activity-card',
  standalone: false,
  templateUrl: './order-activity-card.component.html',
  styles: [':host { display: block; }']
})
export class OrderActivityCardComponent {
  @Input({ required: true }) order!: Order;
}
