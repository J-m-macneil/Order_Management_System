import { Component, computed, input } from '@angular/core';

import { OrderByStatus } from '../../../core/models/dashboard.models';
import { OrderStatus } from '../../../core/models/order-status.enum';

interface StatusDetails {
  id: OrderStatus;
  cssClass: string;
}

@Component({
  selector: 'app-order-status-overview',
  standalone: false,
  templateUrl: './order-status-overview.component.html',
  styleUrls: ['./order-status-overview.component.css']
})
export class OrderStatusOverviewComponent {
  readonly statuses = input<OrderByStatus[]>([]);

  readonly statusItems = computed(() => this.statuses().map(status => ({
    ...status,
    ...(statusDetails[status.status] ?? statusDetails['Draft'])
  })));
}

const statusDetails: Record<string, StatusDetails> = {
  'Draft': { id: OrderStatus.Draft, cssClass: 'status-draft' },
  'Submitted': { id: OrderStatus.Submitted, cssClass: 'status-submitted' },
  'Pending Review': { id: OrderStatus.PendingReview, cssClass: 'status-pending-review' },
  'Approved': { id: OrderStatus.Approved, cssClass: 'status-approved' },
  'In Processing': { id: OrderStatus.InProcessing, cssClass: 'status-processing' },
  'Awaiting Dispatch': { id: OrderStatus.AwaitingDispatch, cssClass: 'status-awaiting-dispatch' },
  'Completed': { id: OrderStatus.Completed, cssClass: 'status-completed' },
  'Failed': { id: OrderStatus.Failed, cssClass: 'status-failed' },
  'Cancelled': { id: OrderStatus.Cancelled, cssClass: 'status-cancelled' }
};
