import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Order } from '../../../core/models/order.model';
import { OrderStatus } from '../../../core/models/order-status.enum';

@Component({
  selector: 'app-order-status-strip',
  standalone: false,
  templateUrl: './order-status-strip.component.html',
  styles: [':host { display: block; }']
})
export class OrderStatusStripComponent {
  @Input({ required: true }) order!: Order;
  @Input() isDiscardingDraft = false;

  @Output() discardDraft = new EventEmitter<void>();

  canEditOrder(): boolean {
    return this.order.orderStatusId === OrderStatus.Draft;
  }

  getEditLockMessage(): string {
    if (this.order.orderStatusId === OrderStatus.Draft) {
      return 'Draft orders can be edited before submission.';
    }

    if (this.order.orderStatusId === OrderStatus.Submitted ||
        this.order.orderStatusId === OrderStatus.PendingReview) {
      return 'Return this order to Draft before making changes.';
    }

    return 'This order is locked. Cancel it and create a replacement if changes are required.';
  }
}
