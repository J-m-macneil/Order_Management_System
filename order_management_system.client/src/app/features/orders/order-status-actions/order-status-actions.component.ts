import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AllowedStatus } from '../../../core/models/allowed-status.model';
import { OrderStatus } from '../../../core/models/order-status.enum';

@Component({
  selector: 'app-order-status-actions',
  standalone: false,
  templateUrl: './order-status-actions.component.html',
  styles: [':host { display: block; }']
})
export class OrderStatusActionsComponent {
  @Input() allowedStatuses: AllowedStatus[] = [];
  @Input() currentStatusId: OrderStatus | number | null = null;
  @Input() isChangingStatus = false;

  @Output() statusChange = new EventEmitter<number>();

  onStatusChange(statusId: number): void {
    this.statusChange.emit(statusId);
  }

  getStatusButtonClass(statusName: string): string {
    switch (statusName) {
      case 'Submitted':
      case 'Approved':
        return 'bg-blue-600 hover:bg-blue-700';

      case 'In Processing':
      case 'Awaiting Dispatch':
        return 'bg-amber-500 hover:bg-amber-600';

      case 'Completed':
        return 'bg-green-600 hover:bg-green-700';

      case 'Failed':
      case 'Cancelled':
        return 'bg-red-600 hover:bg-red-700';

      default:
        return 'bg-slate-600 hover:bg-slate-700';
    }
  }

  getStatusActionLabel(status: AllowedStatus): string {
    if (status.id === OrderStatus.Draft && this.currentStatusId === OrderStatus.Submitted) {
      return 'Withdraw to Draft';
    }

    if (status.id === OrderStatus.Draft && this.currentStatusId === OrderStatus.PendingReview) {
      return 'Return to Draft';
    }

    return `Move to ${status.name}`;
  }
}
