import { Component, Input } from '@angular/core';
import { Order } from '../../../core/models/order.model';

@Component({
  selector: 'app-order-information-card',
  standalone: false,
  templateUrl: './order-information-card.component.html',
  styles: [':host { display: block; }']
})
export class OrderInformationCardComponent {
  @Input({ required: true }) order!: Order;
}
