export interface OrderItem {
  orderItemId: number;
  productId: number;
  productName?: string;
  quantity: number;
  unitPrice: number;
  discountPercent: number;
  lineTotal: number;
  notes?: string | null;
}
