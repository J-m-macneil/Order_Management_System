export interface ProductList {
  productId: number;
  sku: string;
  productName: string;

  productCategoryName: string;
  unitOfMeasureName: string;
  hazardClassName: string;

  packSize: string;
  basePrice: number;
  currency: string;

  isRestricted: boolean;
  isActive: boolean;
}
