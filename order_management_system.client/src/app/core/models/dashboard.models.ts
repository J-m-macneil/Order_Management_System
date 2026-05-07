export type MetricType = 'orders' | 'activeOrders' | 'failedOrders' | 'totalValue';

export interface MetricCard {
  label: string;
  value: number;
  type: MetricType;
  color: string;
}

export interface OrderByStatus {
  status: string;
  count: number;
  color: string;
}

export interface TopCustomer {
  name: string;
  initials: string;
  orders: number;
  bgColor: string;
}

export interface RecentFailure {
  orderNumber: string;
  customer: string;
  reason: string;
  date: string;
}

export interface PriorityOrder {
  orderNumber: string;
  customer: string;
  priority: string;
  dueDate: string;
}
