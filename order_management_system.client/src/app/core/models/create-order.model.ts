import { CreateOrderItem } from './create-order-item.model';

export interface CreateOrder {
  customerId: number;
  deliveryAddressId: number;
  billingAddressId: number;

  warehouseId: number;
  carrierId?: number | null;
  projectId?: number | null;

  requestedDeliveryDate: string;

  purchaseOrderReference?: string | null;
  specialInstructions?: string | null;
  internalNotes?: string | null;

  isPriorityOrder: boolean;

  items: CreateOrderItem[];
}
