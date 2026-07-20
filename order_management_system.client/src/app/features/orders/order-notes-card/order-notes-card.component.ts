import { Component, Input } from '@angular/core';
import { Order } from '../../../core/models/order.model';

@Component({
  selector: 'app-order-notes-card',
  standalone: false,
  templateUrl: './order-notes-card.component.html',
  styles: [':host { display: block; }']
})
export class OrderNotesCardComponent {
  @Input({ required: true }) order!: Order;
}
