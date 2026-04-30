import { Component } from '@angular/core';

type MetricType = 'orders' | 'activeOrders' | 'failedOrders' | 'totalValue';

interface MetricCard {
  label: string;
  value: number;
  type: MetricType;
  color: string;
}

interface OrderByStatus {
  status: string;
  count: number;
  color: string;
}

interface TopCustomer {
  name: string;
  initials: string;
  orders: number;
  bgColor: string;
}

interface RecentFailure {
  orderNumber: string;
  customer: string;
  reason: string;
  date: string;
}

interface PriorityOrder {
  orderNumber: string;
  customer: string;
  priority: string;
  dueDate: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: false,
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})
export class DashboardComponent {
  metrics: MetricCard[] = [
    {
      label: 'Total Orders',
      value: 1243,
      type: 'orders',
      color: 'text-blue-600'
    },
    {
      label: 'Active Orders',
      value: 287,
      type: 'activeOrders',
      color: 'text-emerald-500'
    },
    {
      label: 'Failed Orders',
      value: 12,
      type: 'failedOrders',
      color: 'text-red-500'
    },
    {
      label: 'Total Value',
      value: 145230,
      type: 'totalValue',
      color: 'text-purple-500'
    }
  ];

  ordersByStatus: OrderByStatus[] = [
    { status: 'Draft', count: 45, color: 'bg-slate-200 dark:bg-slate-700' },
    { status: 'Submitted', count: 89, color: 'bg-blue-200 dark:bg-blue-900/30' },
    { status: 'Approved', count: 123, color: 'bg-emerald-200 dark:bg-emerald-900/30' },
    { status: 'Processing', count: 87, color: 'bg-amber-200 dark:bg-amber-900/30' }
  ];

  topCustomers: TopCustomer[] = [
    { name: 'Acme Corp', initials: 'AC', orders: 156, bgColor: 'bg-blue-600' },
    { name: 'ChemTech', initials: 'CT', orders: 134, bgColor: 'bg-purple-600' },
    { name: 'Industrial Solutions', initials: 'IS', orders: 98, bgColor: 'bg-emerald-600' },
    { name: 'GreenChem', initials: 'GC', orders: 87, bgColor: 'bg-amber-600' }
  ];

  recentFailures: RecentFailure[] = [
    {
      orderNumber: 'ORD-2024-1089',
      customer: 'Acme Corp',
      reason: 'Payment declined',
      date: '2024-01-15'
    },
    {
      orderNumber: 'ORD-2024-1087',
      customer: 'TechChemical',
      reason: 'Out of stock',
      date: '2024-01-14'
    }
  ];

  priorityOrders: PriorityOrder[] = [
    {
      orderNumber: 'ORD-2024-1090',
      customer: 'Acme Corp',
      priority: 'Urgent',
      dueDate: '2024-01-18'
    },
    {
      orderNumber: 'ORD-2024-1091',
      customer: 'ChemTech',
      priority: 'High',
      dueDate: '2024-01-19'
    }
  ];

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
    return new Intl.NumberFormat('en-UK', {
      style: 'currency',
      currency: 'GBP',
      maximumFractionDigits: 0
    }).format(value);
  }
}
