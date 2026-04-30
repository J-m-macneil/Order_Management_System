export interface OrderStatusHistory {
  orderStatusHistoryId: number;
  fromStatusName?: string | null;
  toStatusName: string;
  changedByUserName: string;
  changedAt: string;
  reason?: string | null;
}
