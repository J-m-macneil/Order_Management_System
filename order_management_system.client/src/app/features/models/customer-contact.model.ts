export interface CustomerContact {
  customerContactId: number;
  customerId: number;
  name: string;
  jobTitle?: string | null;
  email: string;
  phone?: string | null;
  isPrimary: boolean;
}

export interface CreateCustomerContactRequest {
  name: string;
  jobTitle?: string | null;
  email: string;
  phone?: string | null;
  isPrimary: boolean;
}

export interface UpdateCustomerContactRequest extends CreateCustomerContactRequest { }
