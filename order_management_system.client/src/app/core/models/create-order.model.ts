import { CreateOrderItem } from './create-order-item.model';

export interface CreateOrder {
  customerId: number;
  deliveryAddressId: number;
  billingAddressId: number;
  createdByUserId: number;
  requestedDeliveryDate: string;
  isPriorityOrder: boolean;
  items: CreateOrderItem[];
}
