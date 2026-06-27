import { Component, Input } from '@angular/core';
import { Order } from '../../../core/models/order.model';

@Component({
  selector: 'app-order-logistics-card',
  standalone: false,
  templateUrl: './order-logistics-card.component.html',
  styles: [':host { display: block; }']
})
export class OrderLogisticsCardComponent {
  @Input({ required: true }) order!: Order;
}
