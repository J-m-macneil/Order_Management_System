import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { OrdersService } from '../../../core/services/orders.service';
import { Order } from '../../../core/models/order.model';
import { AllowedStatus } from '../../../core/models/allowed-status.model';
import { OrderStatus } from '../../../core/models/order-status.enum';
import { OrderStatusHistory } from '../../../core/models/order-status-history.model';
import { getApiErrorMessage } from '../../../core/utils/api-error-message';
import {
  ConfirmationModalState,
  ConfirmationModalVariant,
  PendingConfirmationAction
} from './order-detail-confirmation-modal.model';

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
  isDiscardingDraft = false;
  errorMessage = '';

  confirmationModal: ConfirmationModalState = this.createDefaultConfirmationModal();
  pendingConfirmationAction: PendingConfirmationAction | null = null;
  pendingStatus: OrderStatus | null = null;

  private orderId = 0;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private ordersService: OrdersService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    this.orderId = idParam ? Number(idParam) : 0;

    if (!this.orderId) {
      this.errorMessage = 'Invalid order id.';
      return;
    }

    this.refresh();
  }

  refresh(): void {
    this.loadOrder();
    this.loadAllowedStatuses();
    this.loadHistory();
  }

  private loadOrder(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.ordersService.getOrderById(this.orderId).subscribe({
      next: (data) => {
        this.order = data;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.errorMessage = 'Failed to load order details.';
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  private loadAllowedStatuses(): void {
    this.ordersService.getAllowedStatuses(this.orderId).subscribe({
      next: (statuses) => {
        this.allowedStatuses = statuses;
        this.cdr.markForCheck();
      },
      error: () => {
        this.allowedStatuses = [];
        this.cdr.markForCheck();
      }
    });
  }

  private loadHistory(): void {
    this.ordersService.getOrderHistory(this.orderId).subscribe({
      next: (data) => {
        this.history = data;
        this.cdr.markForCheck();
      },
      error: () => {
        this.history = [];
        this.cdr.markForCheck();
      }
    });
  }

  requestStatusChange(statusId: number): void {
    if (!this.order || this.isChangingStatus) {
      return;
    }

    const status = statusId as OrderStatus;

    if (this.requiresReason(status)) {
      this.openStatusReasonModal(status);
      return;
    }

    this.executeStatusChange(status);
  }

  onConfirmationConfirm(reason?: string): void {
    if (this.pendingConfirmationAction === 'discardDraft') {
      this.confirmDiscardDraft();
      return;
    }

    if (this.pendingConfirmationAction === 'status') {
      this.confirmStatusChange(reason);
    }
  }

  onConfirmationCancel(): void {
    this.closeConfirmationModal();
  }

  private executeStatusChange(status: OrderStatus, reason?: string): void {
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
          getApiErrorMessage(err, 'Failed to change order status.');
        this.isChangingStatus = false;
        this.cdr.markForCheck();
      }
    });
  }

  requestDiscardDraft(): void {
    if (!this.order || this.isDiscardingDraft) {
      return;
    }

    this.openDiscardDraftModal();
  }

  get restrictedItems() {
    return this.order?.items.filter(item => item.isRestricted) ?? [];
  }

  private executeDiscardDraft(): void {
    if (!this.order || this.isDiscardingDraft) {
      return;
    }

    this.isDiscardingDraft = true;
    this.errorMessage = '';

    this.ordersService.discardDraftOrder(this.order.orderId).subscribe({
      next: () => {
        this.isDiscardingDraft = false;
        this.router.navigate(['/orders']);
      },
      error: (err) => {
        this.errorMessage = getApiErrorMessage(err, 'Failed to discard draft order.');
        this.isDiscardingDraft = false;
        this.cdr.markForCheck();
      }
    });
  }

  private confirmDiscardDraft(): void {
    this.closeConfirmationModal();
    this.executeDiscardDraft();
  }

  private confirmStatusChange(reason?: string): void {
    if (!this.pendingStatus) {
      return;
    }

    const status = this.pendingStatus;
    this.closeConfirmationModal();
    this.executeStatusChange(status, reason);
  }

  private requiresReason(status: OrderStatus): boolean {
    return status === OrderStatus.Draft ||
      status === OrderStatus.Failed ||
      status === OrderStatus.Cancelled;
  }

  private openStatusReasonModal(status: OrderStatus): void {
    this.pendingStatus = status;
    this.pendingConfirmationAction = 'status';
    this.errorMessage = '';
    this.confirmationModal = {
      isOpen: true,
      title: this.getReasonModalTitle(status),
      message: '',
      confirmText: 'Confirm',
      variant: this.getReasonModalVariant(status),
      requireReason: true,
      reasonPlaceholder: this.getReasonModalPlaceholder(status)
    };
  }

  private openDiscardDraftModal(): void {
    if (!this.order) {
      return;
    }

    this.pendingConfirmationAction = 'discardDraft';
    this.pendingStatus = null;
    this.confirmationModal = {
      isOpen: true,
      title: 'Discard Draft Order',
      message: `Discard draft order ${this.order.orderNumber}? This will remove it from the active orders list.`,
      confirmText: 'Discard Draft',
      variant: 'danger',
      requireReason: false,
      reasonPlaceholder: ''
    };
  }

  private closeConfirmationModal(): void {
    this.confirmationModal = this.createDefaultConfirmationModal();
    this.pendingStatus = null;
    this.pendingConfirmationAction = null;
  }

  private createDefaultConfirmationModal(): ConfirmationModalState {
    return {
      isOpen: false,
      title: 'Confirm Action',
      message: '',
      confirmText: 'Confirm',
      variant: 'default',
      requireReason: false,
      reasonPlaceholder: 'Enter reason...'
    };
  }

  private getReasonModalVariant(status: OrderStatus): ConfirmationModalVariant {
    return status === OrderStatus.Cancelled || status === OrderStatus.Failed
      ? 'danger'
      : 'warning';
  }

  private getReasonModalTitle(status: OrderStatus): string {
    if (status === OrderStatus.Draft) {
      return 'Return Order to Draft';
    }

    if (status === OrderStatus.Cancelled) {
      return 'Cancel Order';
    }

    if (status === OrderStatus.Failed) {
      return 'Fail Order';
    }

    return 'Provide Reason';
  }

  private getReasonModalPlaceholder(status: OrderStatus): string {
    if (status === OrderStatus.Draft) {
      return 'Explain why this order needs to be changed...';
    }

    if (status === OrderStatus.Cancelled) {
      return 'Explain why this order is being cancelled...';
    }

    if (status === OrderStatus.Failed) {
      return 'Explain why this order failed...';
    }

    return 'Enter reason...';
  }
}
