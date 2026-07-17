import { Component, OnInit, signal } from '@angular/core';

import { MetricCard, OrderByStatus, PriorityOrder, RecentFailure, TopCustomer } from '../../core/models/dashboard.models';
import { OrderStatus } from '../../core/models/order-status.enum';
import { DashboardService } from '../../core/services/dashboard.service';

@Component({
  selector: 'app-dashboard',
  standalone: false,
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent implements OnInit {
  readonly metrics = signal<MetricCard[]>([]);
  readonly ordersByStatus = signal<OrderByStatus[]>([]);
  readonly topCustomers = signal<TopCustomer[]>([]);
  readonly recentFailures = signal<RecentFailure[]>([]);
  readonly priorityOrders = signal<PriorityOrder[]>([]);

  readonly isLoading = signal(false);
  readonly errorMessage = signal('');
  readonly failedOrderStatus = OrderStatus.Failed;

  constructor(private dashboardService: DashboardService) { }

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.dashboardService.getMetrics().subscribe({
      next: (data) => {
        const totalOrders = data.metrics.totalOrders;
        const activePercentage = totalOrders > 0
          ? Math.round((data.metrics.activeOrders / totalOrders) * 100)
          : 0;

        this.metrics.set([
          {
            label: 'Total Orders',
            value: totalOrders,
            type: 'orders',
            description: 'All recorded orders'
          },
          {
            label: 'Active Orders',
            value: data.metrics.activeOrders,
            type: 'activeOrders',
            description: `${activePercentage}% of all orders`
          },
          {
            label: 'Failed Orders',
            value: data.metrics.failedOrders,
            type: 'failedOrders',
            description: data.metrics.failedOrders > 0 ? 'Requires attention' : 'No current failures'
          },
          {
            label: 'Total Value',
            value: data.metrics.totalValue,
            type: 'totalValue',
            description: 'Across all orders'
          }
        ]);

        this.ordersByStatus.set(data.ordersByStatus);

        const highestOrderCount = Math.max(...data.topCustomers.map(customer => customer.orders), 1);

        this.topCustomers.set(data.topCustomers.map(customer => ({
          name: customer.name,
          orders: customer.orders,
          orderShare: Math.round((customer.orders / highestOrderCount) * 100)
        })));

        this.recentFailures.set(data.recentFailures);

        this.priorityOrders.set(data.priorityOrders.map(order => ({
          orderId: order.orderId,
          orderNumber: order.orderNumber,
          customer: order.customer,
          priority: order.priority,
          dueDate: order.dueDate
        })));

        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Dashboard metrics error:', error);
        this.errorMessage.set('Failed to load dashboard metrics.');
        this.isLoading.set(false);
      }
    });
  }

}
