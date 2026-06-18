import { apiBaseUrl } from '../config/api-url';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateCustomerRequest } from '../models/create-customer.model';
import { Customer } from '../models/customer.model';
import { UpdateCustomerRequest } from '../models/update-customer.model';
import { Address, CreateAddressRequest, UpdateAddressRequest } from '../models/address.model';
import { CreateCustomerContactRequest, CustomerContact, UpdateCustomerContactRequest } from '../models/customer-contact.model';
import { PagedResult } from '../models/paged-result.model';
import { PaginationQuery } from '../models/pagination-query.model';
import { CustomerSummary } from '../models/customer-summary.model';

@Injectable({
  providedIn: 'root'
})
export class CustomersService {
  private readonly baseUrl = `${apiBaseUrl}/customers`;

  constructor(private http: HttpClient) { }

  getAll(query: PaginationQuery & {
    searchTerm?: string;
    industryType?: string;
    paymentTermsDays?: number | null;
    isActive?: boolean | null;
  }): Observable<PagedResult<Customer>> {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize);

    if (query.searchTerm) {
      params = params.set('searchTerm', query.searchTerm);
    }

    if (query.industryType) {
      params = params.set('industryType', query.industryType);
    }

    if (query.paymentTermsDays) {
      params = params.set('paymentTermsDays', query.paymentTermsDays);
    }

    if (query.isActive !== undefined && query.isActive !== null) {
      params = params.set('isActive', query.isActive);
    }

    return this.http.get<PagedResult<Customer>>(this.baseUrl, { params });
  }

  getSummary(): Observable<CustomerSummary> {
    return this.http.get<CustomerSummary>(`${this.baseUrl}/summary`);
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

  getAddresses(customerId: number): Observable<Address[]> {
    return this.http.get<Address[]>(`${this.baseUrl}/${customerId}/addresses`);
  }

  createAddress(customerId: number, request: CreateAddressRequest): Observable<Address> {
    return this.http.post<Address>(`${this.baseUrl}/${customerId}/addresses`, request);
  }

  updateAddress(customerId: number, addressId: number, request: UpdateAddressRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${customerId}/addresses/${addressId}`, request);
  }

  deleteAddress(customerId: number, addressId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${customerId}/addresses/${addressId}`);
  }

  getContacts(customerId: number): Observable<CustomerContact[]> {
    return this.http.get<CustomerContact[]>(`${this.baseUrl}/${customerId}/contacts`);
  }

  createContact(customerId: number, request: CreateCustomerContactRequest): Observable<CustomerContact> {
    return this.http.post<CustomerContact>(`${this.baseUrl}/${customerId}/contacts`, request);
  }

  updateContact(customerId: number, contactId: number, request: UpdateCustomerContactRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${customerId}/contacts/${contactId}`, request);
  }

  deleteContact(customerId: number, contactId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${customerId}/contacts/${contactId}`);
  }
}
