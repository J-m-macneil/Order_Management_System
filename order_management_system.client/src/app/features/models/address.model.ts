export interface Address {
  addressId: number;
  customerId: number;
  addressType: string;
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
  isPrimary: boolean;
}

export interface CreateAddressRequest {
  addressType: string;
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
  isPrimary: boolean;
}

export interface UpdateAddressRequest extends CreateAddressRequest { }
