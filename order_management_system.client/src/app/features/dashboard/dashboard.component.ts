import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { DashboardService } from '../../core/services/dashboard.service';
import { OrderByStatus, RecentFailure, PriorityOrder } from '../../core/models/dashboard.models';

@Component({
  selector: 'app-dashboard',
  standalone: false,
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent implements OnInit {
  metrics: MetricCard[] = [];
  ordersByStatus: OrderByStatus[] = [];
  topCustomers: TopCustomer[] = [];
  recentFailures: RecentFailure[] = [];
  priorityOrders: PriorityOrder[] = [];

  isLoading = false;
  errorMessage = '';

  constructor(
    private dashboardService: DashboardService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.cdr.detectChanges();

    this.dashboardService.getMetrics().subscribe({
      next: (data) => {
        this.metrics = [
          { label: 'Total Orders', value: data.metrics.totalOrders, type: 'orders', color: 'text-blue-600' },
          { label: 'Active Orders', value: data.metrics.activeOrders, type: 'activeOrders', color: 'text-emerald-500' },
          { label: 'Failed Orders', value: data.metrics.failedOrders, type: 'failedOrders', color: 'text-red-500' },
          { label: 'Total Value', value: data.metrics.totalValue, type: 'totalValue', color: 'text-purple-500' }
        ];

        this.ordersByStatus = data.ordersByStatus.map(x => ({
          status: x.status,
          count: x.count,
          color: this.getStatusColor(x.status)
        }));

        this.topCustomers = data.topCustomers.map((x, index) => ({
          name: x.name,
          initials: this.getInitials(x.name),
          orders: x.orders,
          bgColor: this.getCustomerColor(index)
        }));

        this.recentFailures = data.recentFailures.map(x => ({
          orderNumber: x.orderNumber,
          customer: x.customer,
          reason: x.reason,
          date: this.formatDate(x.date)
        }));

        this.priorityOrders = data.priorityOrders.map(x => ({
          orderNumber: x.orderNumber,
          customer: x.customer,
          priority: x.priority,
          dueDate: this.formatDate(x.dueDate)
        }));

        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Dashboard metrics error:', err);
        this.errorMessage = 'Failed to load dashboard metrics.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  getStatusColor(status: string): string {
    const colors: Record<string, string> = {
      'Draft': 'bg-slate-200 dark:bg-slate-700',
      'Submitted': 'bg-blue-200 dark:bg-blue-900/30',
      'Pending Review': 'bg-purple-200 dark:bg-purple-900/30',
      'Approved': 'bg-emerald-200 dark:bg-emerald-900/30',
      'In Processing': 'bg-amber-200 dark:bg-amber-900/30',
      'Awaiting Dispatch': 'bg-orange-200 dark:bg-orange-900/30',
      'Completed': 'bg-green-200 dark:bg-green-900/30',
      'Failed': 'bg-red-200 dark:bg-red-900/30',
      'Cancelled': 'bg-slate-300 dark:bg-slate-600'
    };

    return colors[status] || 'bg-slate-200 dark:bg-slate-700';
  }

  getCustomerColor(index: number): string {
    const colors = [
      'bg-blue-600',
      'bg-purple-600',
      'bg-emerald-600',
      'bg-amber-600',
      'bg-red-600'
    ];

    return colors[index % colors.length];
  }

  getInitials(name: string): string {
    if (!name) return '—';

    return name
      .split(' ')
      .filter(Boolean)
      .slice(0, 2)
      .map(part => part[0].toUpperCase())
      .join('');
  }

  getPriorityColor(priority: string): string {
    const colors: Record<string, string> = {
      low: 'text-slate-600 dark:text-slate-400',
      medium: 'text-blue-600 dark:text-blue-400',
      high: 'text-amber-600 dark:text-amber-400',
      urgent: 'text-red-600 dark:text-red-400'
    };

    return colors[priority.toLowerCase()] || colors['low'];
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('en-GB', {
      style: 'currency',
      currency: 'GBP',
      maximumFractionDigits: 0
    }).format(value);
  }

  private formatDate(value: string | null): string {
    if (!value) return '—';

    return new Date(value).toLocaleDateString('en-GB', {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    });
  }
}
