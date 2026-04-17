import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

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

export interface CreateCustomerRequest {
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
}

export interface UpdateCustomerRequest {
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
}

@Injectable({
  providedIn: 'root'
})
export class CustomersService {
  private readonly baseUrl = 'https://localhost:7233/api/customers';

  constructor(private http: HttpClient) { }

  getAll(): Observable<Customer[]> {
    return this.http.get<Customer[]>(this.baseUrl);
  }

  getById(id: number): Observable<Customer> {
    return this.http.get<Customer>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateCustomerRequest): Observable<Customer> {
    return this.http.post<Customer>(this.baseUrl, request);
  }

  update(id: number, request: UpdateCustomerRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
