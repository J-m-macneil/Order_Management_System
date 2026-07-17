import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { OrdersService } from '../../core/services/orders.service';
import { Order } from '../../core/models/order.model';

@Component({
  selector: 'app-orders',
  standalone: false,
  templateUrl: './orders.component.html',
})
export class OrdersComponent implements OnInit {
  readonly orders = signal<Order[]>([]);

  readonly pageNumber = signal(1);
  readonly pageSize = signal(25);
  readonly totalCount = signal(0);
  readonly totalPages = signal(0);
  readonly hasPreviousPage = signal(false);
  readonly hasNextPage = signal(false);

  readonly isLoading = signal(false);
  readonly errorMessage = signal('');

  searchTerm = '';
  priorityFilter = '';
  restrictedFilter = '';
  statusFilter = '';
  requestedDeliveryFrom = '';
  requestedDeliveryTo = '';
  createdFrom = '';
  createdTo = '';

  filtersVisible = false;

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
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.statusFilter = this.getStatusFilterFromRoute();
    this.priorityFilter = this.getPriorityFilterFromRoute();
    this.loadOrders();
  }

  loadOrders(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    const request = {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      searchTerm: this.searchTerm.trim() || undefined,
      orderStatusId: this.statusFilter ? Number(this.statusFilter) : null,
      isPriorityOrder: this.getPriorityFilterValue(),
      hasRestrictedItems: this.getRestrictedFilterValue(),
      requestedDeliveryFrom: this.requestedDeliveryFrom || undefined,
      requestedDeliveryTo: this.requestedDeliveryTo || undefined,
      createdFrom: this.createdFrom || undefined,
      createdTo: this.createdTo || undefined
    };

    this.ordersService.getOrders(request)
      .subscribe({
        next: (data) => {
          this.orders.set(data.items);
          this.pageNumber.set(data.pageNumber);
          this.pageSize.set(data.pageSize);
          this.totalCount.set(data.totalCount);
          this.totalPages.set(data.totalPages);
          this.hasPreviousPage.set(data.hasPreviousPage);
          this.hasNextPage.set(data.hasNextPage);
          this.isLoading.set(false);
        },
        error: () => {
          this.errorMessage.set('Failed to load orders.');
          this.isLoading.set(false);
        }
      });
  }

  toggleFilters(): void {
    this.filtersVisible = !this.filtersVisible;
  }

  applyFilters(): void {
    this.pageNumber.set(1);
    this.loadOrders();
  }

  onStatusFilterChange(): void {
    this.updateFilterQueryParameters();
    this.applyFilters();
  }

  onPriorityFilterChange(): void {
    this.updateFilterQueryParameters();
    this.applyFilters();
  }

  clearFilters(): void {
    this.priorityFilter = '';
    this.restrictedFilter = '';
    this.statusFilter = '';
    this.requestedDeliveryFrom = '';
    this.requestedDeliveryTo = '';
    this.createdFrom = '';
    this.createdTo = '';
    this.updateFilterQueryParameters();
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
    return isPriority ? 'app-badge app-badge--warning' : 'app-badge app-badge--neutral';
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

  private getRestrictedFilterValue(): boolean | null {
    if (this.restrictedFilter === 'restricted') {
      return true;
    }

    if (this.restrictedFilter === 'standard') {
      return false;
    }

    return null;
  }

  private getStatusFilterFromRoute(): string {
    const status = this.route.snapshot.queryParamMap.get('status') ?? '';

    return this.orderStatuses.some(option => String(option.id) === status)
      ? status
      : '';
  }

  private getPriorityFilterFromRoute(): string {
    const priority = this.route.snapshot.queryParamMap.get('priority') ?? '';

    return priority === 'priority' || priority === 'standard'
      ? priority
      : '';
  }

  private updateFilterQueryParameters(): void {
    void this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        status: this.statusFilter || null,
        priority: this.priorityFilter || null
      },
      queryParamsHandling: 'merge',
      replaceUrl: true
    });
  }

  get activeFilterCount(): number {
    return [
      this.priorityFilter,
      this.restrictedFilter,
      this.statusFilter,
      this.requestedDeliveryFrom,
      this.requestedDeliveryTo,
      this.createdFrom,
      this.createdTo
    ].filter(Boolean).length;
  }

  onPageChange(pageNumber: number): void {
    this.pageNumber.set(pageNumber);
    this.loadOrders();
  }

  onPageSizeChange(value: number): void {
    this.pageSize.set(value);
    this.pageNumber.set(1);
    this.loadOrders();
  }
}
