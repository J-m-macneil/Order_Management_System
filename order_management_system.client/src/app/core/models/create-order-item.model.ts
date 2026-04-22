export interface CreateOrderItem {
  productId: number;

  quantity: number;
  unitPrice: number;
  discountPercent: number;

  notes?: string;
}
