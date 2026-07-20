export interface PricingTier {
  pricingTierId: number;
  name: string;
  discountPercent: number;
  priorityProcessing: boolean;
  description: string;
}
