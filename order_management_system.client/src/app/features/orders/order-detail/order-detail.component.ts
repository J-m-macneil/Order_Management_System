import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { OrdersService } from '../../../core/services/orders.service';
import { Order } from '../../../core/models/order.model';
import { AllowedStatus } from '../../../core/models/allowed-status.model';
import { OrderStatus } from '../../../core/models/order-status.enum';
import { OrderStatusHistory } from '../../../core/models/order-status-history.model';

@Component({
  selector: 'app-order-detail',
  standalone: false,
  templateUrl: './order-detail.component.html',
  styleUrls: ['./order-detail.component.css']
})
export class OrderDetailComponent implements OnInit {
  order: Order | null = null;
  allowedStatuses: AllowedStatus[] = [];
  history: OrderStatusHistory[] = [];

  isLoading = false;
  isChangingStatus = false;
  errorMessage = '';

  showReasonModal = false;
  pendingStatus: OrderStatus | null = null;

  private orderId = 0;

  constructor(
    private route: ActivatedRoute,
    private ordersService: OrdersService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    this.orderId = idParam ? Number(idParam) : 0;

    if (!this.orderId) {
      this.errorMessage = 'Invalid order id.';
      this.cdr.detectChanges();
      return;
    }

    this.refresh();
  }

  refresh(): void {
    this.loadOrder(this.orderId);
    this.loadAllowedStatuses(this.orderId);
    this.loadHistory(this.orderId);
  }

  loadOrder(orderId: number): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.ordersService.getOrderById(orderId).subscribe({
      next: (data) => {
        this.order = data;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Failed to load order details.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadAllowedStatuses(orderId: number): void {
    this.ordersService.getAllowedStatuses(orderId).subscribe({
      next: (statuses) => {
        this.allowedStatuses = statuses;
        this.cdr.detectChanges();
      },
      error: () => {
        this.allowedStatuses = [];
        this.cdr.detectChanges();
      }
    });
  }

  loadHistory(orderId: number): void {
    this.ordersService.getOrderHistory(orderId).subscribe({
      next: (data) => {
        this.history = data;
        this.cdr.detectChanges();
      },
      error: () => {
        this.history = [];
        this.cdr.detectChanges();
      }
    });
  }

  changeStatus(statusId: number): void {
    if (!this.order || this.isChangingStatus) {
      return;
    }

    const status = statusId as OrderStatus;

    if (status === OrderStatus.Failed || status === OrderStatus.Cancelled) {
      this.pendingStatus = status;
      this.showReasonModal = true;
      this.errorMessage = '';
      this.cdr.detectChanges();
      return;
    }

    this.executeStatusChange(status);
  }

  onReasonConfirm(reason: string): void {
    if (!this.pendingStatus) {
      return;
    }

    this.showReasonModal = false;
    this.executeStatusChange(this.pendingStatus, reason);
    this.pendingStatus = null;
  }

  onReasonCancel(): void {
    this.showReasonModal = false;
    this.pendingStatus = null;
    this.cdr.detectChanges();
  }

  executeStatusChange(status: OrderStatus, reason?: string): void {
    if (!this.order) {
      return;
    }

    this.isChangingStatus = true;
    this.errorMessage = '';

    this.ordersService.changeStatus(this.order.orderId, status, reason).subscribe({
      next: () => {
        this.isChangingStatus = false;
        this.refresh();
      },
      error: (err) => {
        this.errorMessage =
          err.error?.message || 'Failed to change order status.';
        this.isChangingStatus = false;
        this.cdr.detectChanges();
      }
    });
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
}
