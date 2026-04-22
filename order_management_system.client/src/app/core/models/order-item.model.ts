export interface OrderItem {
  orderItemId: number;
  productId: number;

  quantity: number;
  unitPrice: number;
  discountPercent: number;
  lineTotal: number;

  notes?: string;
}
