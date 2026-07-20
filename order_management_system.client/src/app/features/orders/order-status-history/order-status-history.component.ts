import { Component, Input } from '@angular/core';
import { OrderStatusHistory } from '../../../core/models/order-status-history.model';

@Component({
  selector: 'app-order-status-history',
  standalone: false,
  templateUrl: './order-status-history.component.html',
  styles: [':host { display: block; }']
})
export class OrderStatusHistoryComponent {
  @Input() history: OrderStatusHistory[] = [];
}
