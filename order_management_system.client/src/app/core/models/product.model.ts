export interface Product {
  productId: number;
  sku: string;
  productName: string;
  description?: string | null;

  productCategoryId: number;
  unitOfMeasureId: number;
  packSize: string;

  basePrice: number;
  currency: string;

  hazardClassId: number;
  unNumber?: string | null;
  storageRequirement?: string | null;

  requiresSds: boolean;
  isRestricted: boolean;
  isActive: boolean;
}
