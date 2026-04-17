export interface Customer {
  customerId: number;
  accountNumber: string;
  companyName: string;
  industryType: string;
  mainContactName: string;
  mainContactEmail: string;
  mainContactPhone: string;
  billingAddressId: number | null;
  defaultDeliveryAddressId: number | null;
  pricingTierId: number;
  paymentTermsDays: number;
  creditLimit: number;
  isActive: boolean;
  createdAt: string;
}
