export type MetricType = 'orders' | 'activeOrders' | 'failedOrders' | 'totalValue';

export interface MetricCard {
  label: string;
  value: number;
  type: MetricType;
  description: string;
}

export interface OrderByStatus {
  status: string;
  count: number;
}

export interface TopCustomer {
  name: string;
  orders: number;
  orderShare: number;
}

export interface RecentFailure {
  orderId: number;
  orderNumber: string;
  customer: string;
  reason: string;
  date: string;
  requiresAction: boolean;
}

export interface PriorityOrder {
  orderId: number;
  orderNumber: string;
  customer: string;
  priority: string;
  dueDate: string;
}
