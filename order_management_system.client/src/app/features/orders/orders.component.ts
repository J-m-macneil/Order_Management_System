import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { OrdersService } from '../../core/services/orders.service';
import { Order } from '../../core/models/order.model';

@Component({
  selector: 'app-orders',
  standalone: false,
  templateUrl: './orders.component.html',
})
export class OrdersComponent implements OnInit {
  orders: Order[] = [];

  pageNumber = 1;
  pageSize = 25;
  totalCount = 0;
  totalPages = 0;
  hasPreviousPage = false;
  hasNextPage = false;
  readonly pageSizeOptions = [25, 50, 100];

  isLoading = false;
  errorMessage = '';

  searchTerm = '';
  priorityFilter = '';
  statusFilter = '';
  requestedDeliveryFrom = '';
  requestedDeliveryTo = '';
  createdFrom = '';
  createdTo = '';

  private filtersVisible = false;

  readonly orderStatuses = [
    { id: 1, name: 'Draft', badgeClass: 'app-badge app-badge--neutral' },
    { id: 2, name: 'Submitted', badgeClass: 'app-badge app-badge--info' },
    { id: 3, name: 'Pending Review', badgeClass: 'app-badge app-badge--warning' },
    { id: 4, name: 'Approved', badgeClass: 'app-badge app-badge--success' },
    { id: 5, name: 'In Processing', badgeClass: 'app-badge app-badge--info' },
    { id: 6, name: 'Awaiting Dispatch', badgeClass: 'app-badge app-badge--info' },
    { id: 7, name: 'Completed', badgeClass: 'app-badge app-badge--success' },
    { id: 8, name: 'Failed', badgeClass: 'app-badge app-badge--danger' },
    { id: 9, name: 'Cancelled', badgeClass: 'app-badge app-badge--neutral' }
  ];

  private readonly orderStatusLabels = new Map(
    this.orderStatuses.map(status => [status.id, status.name])
  );

  private readonly orderStatusBadgeClasses = new Map(
    this.orderStatuses.map(status => [status.id, status.badgeClass])
  );

  constructor(
    private ordersService: OrdersService,
    // Required to update loading/table state after backend response in this view.
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.isLoading = true;
    this.errorMessage = '';

    const request = {
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm.trim() || undefined,
      orderStatusId: this.statusFilter ? Number(this.statusFilter) : null,
      isPriorityOrder: this.getPriorityFilterValue(),
      requestedDeliveryFrom: this.requestedDeliveryFrom || undefined,
      requestedDeliveryTo: this.requestedDeliveryTo || undefined,
      createdFrom: this.createdFrom || undefined,
      createdTo: this.createdTo || undefined
    };

    this.ordersService.getOrders(request)
      .subscribe({
        next: (data) => {
          this.orders = data.items;
          this.pageNumber = data.pageNumber;
          this.pageSize = data.pageSize;
          this.totalCount = data.totalCount;
          this.totalPages = data.totalPages;
          this.hasPreviousPage = data.hasPreviousPage;
          this.hasNextPage = data.hasNextPage;
          this.isLoading = false;
          this.cdr.detectChanges();
        },
        error: () => {
          this.errorMessage = 'Failed to load orders.';
          this.isLoading = false;
          this.cdr.detectChanges();
        }
      });
  }

  showFilters(): boolean {
    return this.filtersVisible;
  }

  toggleFilters(): void {
    this.filtersVisible = !this.filtersVisible;
  }

  applyFilters(): void {
    this.pageNumber = 1;
    this.loadOrders();
  }

  clearFilters(): void {
    this.priorityFilter = '';
    this.statusFilter = '';
    this.requestedDeliveryFrom = '';
    this.requestedDeliveryTo = '';
    this.createdFrom = '';
    this.createdTo = '';
    this.applyFilters();
  }

  getPriorityLabel(isPriority: boolean): string {
    return isPriority ? 'High' : 'Standard';
  }

  getStatusLabel(statusId: number): string {
    return this.orderStatusLabels.get(statusId) ?? 'Unknown';
  }

  getStatusColor(statusId: number): string {
    return this.orderStatusBadgeClasses.get(statusId) ?? 'app-badge app-badge--neutral';
  }

  getPriorityBadgeClass(isPriority: boolean): string {
    return isPriority ? 'app-badge app-badge--warning' : 'app-badge app-badge--info';
  }

  private getPriorityFilterValue(): boolean | null {
    if (this.priorityFilter === 'priority') {
      return true;
    }

    if (this.priorityFilter === 'standard') {
      return false;
    }

    return null;
  }

  get activeFilterCount(): number {
    return [
      this.priorityFilter,
      this.statusFilter,
      this.requestedDeliveryFrom,
      this.requestedDeliveryTo,
      this.createdFrom,
      this.createdTo
    ].filter(Boolean).length;
  }

  goToPreviousPage(): void {
    if (!this.hasPreviousPage) {
      return;
    }

    this.pageNumber--;
    this.loadOrders();
  }

  goToNextPage(): void {
    if (!this.hasNextPage) {
      return;
    }

    this.pageNumber++;
    this.loadOrders();
  }

  onPageSizeChange(value: number): void {
    if (!this.pageSizeOptions.includes(value)) {
      return;
    }

    this.pageSize = value;
    this.pageNumber = 1;
    this.loadOrders();
  }
}
