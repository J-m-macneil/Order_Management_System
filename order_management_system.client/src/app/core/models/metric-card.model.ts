interface MetricCard {
  label: string;
  value: number;
  type: MetricType;
  color: string;
}

type MetricType = 'orders' | 'activeOrders' | 'failedOrders' | 'totalValue';
