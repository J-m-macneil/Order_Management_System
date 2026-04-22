import { OrderItem } from './order-item.model';

export interface Order {
  orderId: number;
  orderNumber: string;

  customerId: number;
  deliveryAddressId: number;
  billingAddressId: number;

  createdByUserId: number;
  assignedToUserId?: number;

  requestedDeliveryDate: string;
  submittedAt?: string;

  createdAt: string;
  updatedAt: string;

  currency: string;

  subtotal: number;
  discountAmount: number;
  taxAmount: number;
  totalAmount: number;

  purchaseOrderReference?: string;
  specialInstructions?: string;
  internalNotes?: string;
  failureReason?: string;

  isPriorityOrder: boolean;

  items: OrderItem[];
}
