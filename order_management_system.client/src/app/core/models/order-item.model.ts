export interface OrderItem {
  orderItemId: number;
  productId: number;
  productName?: string;
  productSku?: string | null;
  packSize?: string | null;
  unNumber?: string | null;
  requiresSds: boolean;
  isRestricted: boolean;
  quantity: number;
  unitPrice: number;
  discountPercent: number;
  lineTotal: number;
  notes?: string | null;
}
