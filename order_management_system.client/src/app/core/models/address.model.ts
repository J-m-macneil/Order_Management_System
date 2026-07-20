export type AddressType = 'Billing' | 'DeliverySite' | 'WarehousePartner';

export interface Address {
  addressId: number;
  customerId: number;
  addressType: AddressType;
  siteName: string;
  line1: string;
  line2?: string | null;
  city: string;
  county?: string | null;
  postcode: string;
  country: string;
  contactName?: string | null;
  contactPhone?: string | null;
  deliveryInstructions?: string | null;
}

export interface CreateAddressRequest {
  addressType: AddressType;
  siteName: string;
  line1: string;
  line2?: string | null;
  city: string;
  county?: string | null;
  postcode: string;
  country: string;
  contactName?: string | null;
  contactPhone?: string | null;
  deliveryInstructions?: string | null;
}

export interface UpdateAddressRequest extends CreateAddressRequest { }
