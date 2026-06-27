import { Address } from './address.model';
import { OrderItem } from './order-item.model';

export interface Order {
  orderId: number;
  orderNumber: string;

  customerId: number;
  customerName?: string;

  deliveryAddressId: number;
  billingAddressId: number;
  deliveryAddress?: Address | null;
  billingAddress?: Address | null;

  warehouseId: number;
  warehouseName?: string;

  carrierId?: number | null;
  carrierName?: string | null;

  projectId?: number | null;
  projectName?: string | null;

  orderStatusId: number;
  orderStatusName?: string;

  createdByUserId: number;
  assignedToUserId?: number | null;

  requestedDeliveryDate: string;
  submittedAt?: string | null;

  createdAt: string;
  updatedAt: string;

  currency: string;

  subtotal: number;
  discountAmount: number;
  taxAmount: number;
  totalAmount: number;

  purchaseOrderReference?: string | null;
  specialInstructions?: string | null;
  internalNotes?: string | null;
  failureReason?: string | null;

  isPriorityOrder: boolean;
  failedProcessingJobCount: number;

  items: OrderItem[];
}
