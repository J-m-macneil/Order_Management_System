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
  filteredOrders: Order[] = [];

  pageNumber = 1;
  pageSize = 25;
  totalCount = 0;
  totalPages = 0;
  hasPreviousPage = false;
  hasNextPage = false;
  pageSizeOptions = [25, 50, 100];

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

  orderStatuses = [
    { id: 1, name: 'Draft' },
    { id: 2, name: 'Submitted' },
    { id: 3, name: 'Pending Review' },
    { id: 4, name: 'Approved' },
    { id: 5, name: 'In Processing' },
    { id: 6, name: 'Awaiting Dispatch' },
    { id: 7, name: 'Completed' },
    { id: 8, name: 'Failed' },
    { id: 9, name: 'Cancelled' }
  ];

  statusLabels: Record<number | string, string> = {
    1: 'Draft',
    2: 'Submitted',
    3: 'Pending Review',
    4: 'Approved',
    5: 'In Processing',
    6: 'Awaiting Dispatch',
    7: 'Completed',
    8: 'Failed',
    9: 'Cancelled'
  };

  statusColors: Record<number | string, string> = {
    1: 'bg-slate-200 dark:bg-slate-700 text-slate-800 dark:text-slate-200',
    2: 'bg-blue-100 dark:bg-blue-900/20 text-blue-700 dark:text-blue-400',
    3: 'bg-amber-100 dark:bg-amber-900/20 text-amber-700 dark:text-amber-400',
    4: 'bg-emerald-100 dark:bg-emerald-900/20 text-emerald-700 dark:text-emerald-400',
    5: 'bg-purple-100 dark:bg-purple-900/20 text-purple-700 dark:text-purple-400',
    6: 'bg-blue-100 dark:bg-blue-900/20 text-blue-700 dark:text-blue-400',
    7: 'bg-emerald-100 dark:bg-emerald-900/20 text-emerald-700 dark:text-emerald-400',
    8: 'bg-red-100 dark:bg-red-900/20 text-red-700 dark:text-red-400',
    9: 'bg-slate-200 dark:bg-slate-700 text-slate-800 dark:text-slate-200'
  };

  priorityColors: Record<string, string> = {
    low: 'text-slate-600 dark:text-slate-400',
    medium: 'text-blue-600 dark:text-blue-400',
    high: 'text-amber-600 dark:text-amber-400',
    urgent: 'text-red-600 dark:text-red-400'
  };

  constructor(
    private ordersService: OrdersService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.ordersService.getOrders({
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm.trim() || undefined,
      orderStatusId: this.statusFilter ? Number(this.statusFilter) : null,
      isPriorityOrder: this.getPriorityFilterValue(),
      requestedDeliveryFrom: this.requestedDeliveryFrom || undefined,
      requestedDeliveryTo: this.requestedDeliveryTo || undefined,
      createdFrom: this.createdFrom || undefined,
      createdTo: this.createdTo || undefined
    })
    .subscribe({
      next: (data) => {
        this.orders = data.items;
        this.pageNumber = data.pageNumber;
        this.pageSize = data.pageSize;
        this.totalCount = data.totalCount;
        this.totalPages = data.totalPages;
        this.hasPreviousPage = data.hasPreviousPage;
        this.hasNextPage = data.hasNextPage;
        this.initialiseOrdersList();
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

  private initialiseOrdersList(): void {
    this.filteredOrders = this.orders;
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

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('en-GB', {
      style: 'currency',
      currency: 'GBP',
      maximumFractionDigits: 2
    }).format(value);
  }

  getPriorityLabel(isPriority: boolean): string {
    return isPriority ? 'High' : 'Standard';
  }

  getStatusColor(statusId: number | string): string {
    return this.statusColors[statusId] || this.statusColors[1];
  }

  getPriorityColor(isPriority: boolean): string {
    return isPriority ? this.priorityColors['high'] : this.priorityColors['medium'];
  }

  getOrderStatusId(order: Order): number {
    return order.orderStatusId;
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
