import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { OrdersService } from '../../core/services/orders.service';

@Component({
  selector: 'app-orders',
  standalone: false,
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.css'],
})
export class OrdersComponent implements OnInit {
  orders: any[] = [];
  filteredOrders: any[] = [];
  isLoading = false;
  errorMessage = '';

  searchTerm = '';
  priorityFilter = '';
  statusFilter = '';

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

    this.ordersService.getOrders().subscribe({
      next: (data) => {
        this.orders = data;
        this.applyFilters();
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

  applyFilters(): void {
    const term = this.searchTerm.trim().toLowerCase();

    this.filteredOrders = this.orders.filter(order => {
      const matchesSearch =
        !term ||
        order.orderNumber?.toLowerCase().includes(term) ||
        order.customerName?.toLowerCase().includes(term) ||
        order.customerId?.toString().includes(term) ||
        order.purchaseOrderReference?.toLowerCase().includes(term);

      const matchesPriority =
        !this.priorityFilter ||
        (this.priorityFilter === 'priority' && order.isPriorityOrder) ||
        (this.priorityFilter === 'standard' && !order.isPriorityOrder);

      const matchesStatus =
        !this.statusFilter ||
        order.orderStatusName === this.statusFilter ||
        order.orderStatusId?.toString() === this.statusFilter;

      return matchesSearch && matchesPriority && matchesStatus;
    });

    this.cdr.detectChanges();
  }
}
