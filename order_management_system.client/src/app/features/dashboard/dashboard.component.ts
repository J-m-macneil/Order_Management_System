import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { DashboardService } from '../../core/services/dashboard.service';
import { MetricCard, OrderByStatus, RecentFailure, PriorityOrder, TopCustomer } from '../../core/models/dashboard.models';

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
      'Draft': 'bg-slate-300 dark:bg-slate-400',
      'Submitted': 'bg-blue-300 dark:bg-blue-400',
      'Pending Review': 'bg-violet-300 dark:bg-violet-400',
      'Approved': 'bg-emerald-300 dark:bg-emerald-400',
      'In Processing': 'bg-amber-300 dark:bg-amber-400',
      'Awaiting Dispatch': 'bg-orange-300 dark:bg-orange-400',
      'Completed': 'bg-teal-300 dark:bg-teal-400',
      'Failed': 'bg-red-300 dark:bg-red-400',
      'Cancelled': 'bg-zinc-300 dark:bg-zinc-400'
    };

    return colors[status] || 'bg-slate-300 dark:bg-slate-400';
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
