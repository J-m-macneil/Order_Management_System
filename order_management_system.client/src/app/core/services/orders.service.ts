import { apiBaseUrl } from '../config/api-url';
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateOrder } from '../models/create-order.model';
import { Order } from '../models/order.model';
import { AllowedStatus } from '../models/allowed-status.model';
import { ChangeOrderStatusRequest } from '../models/change-order-status-request.model';
import { OrderStatus } from '../models/order-status.enum';
import { OrderStatusHistory } from '../models/order-status-history.model';
import { ProcessingJob } from '../models/processing-job.model';
import { PagedResult } from '../models/paged-result.model';
import { PaginationQuery } from '../models/pagination-query.model';
import { ProductList } from '../models/product-list.model';

@Injectable({
  providedIn: 'root'
})
export class OrdersService {
  private readonly baseUrl = `${apiBaseUrl}/orders`;
    apiUrl: any;

  constructor(private http: HttpClient) { }

  getOrders(query: PaginationQuery & {
    searchTerm?: string;
    orderStatusId?: number | null;
    isPriorityOrder?: boolean | null;
    requestedDeliveryFrom?: string;
    requestedDeliveryTo?: string;
    createdFrom?: string;
    createdTo?: string;
  }): Observable<PagedResult<Order>> {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize);

    if (query.searchTerm) {
      params = params.set('searchTerm', query.searchTerm);
    }

    if (query.orderStatusId) {
      params = params.set('orderStatusId', query.orderStatusId);
    }

    if (query.isPriorityOrder !== undefined && query.isPriorityOrder !== null) {
      params = params.set('isPriorityOrder', query.isPriorityOrder);
    }

    if (query.requestedDeliveryFrom) {
      params = params.set('requestedDeliveryFrom', query.requestedDeliveryFrom);
    }

    if (query.requestedDeliveryTo) {
      params = params.set('requestedDeliveryTo', query.requestedDeliveryTo);
    }

    if (query.createdFrom) {
      params = params.set('createdFrom', query.createdFrom);
    }

    if (query.createdTo) {
      params = params.set('createdTo', query.createdTo);
    }

    return this.http.get<PagedResult<Order>>(this.baseUrl, { params });
  }

  createOrder(order: CreateOrder): Observable<number> {
    return this.http.post<number>(this.baseUrl, order);
  }

  updateOrder(orderId: number, order: CreateOrder): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${orderId}`, order);
  }

  getOrderById(id: number): Observable<Order> {
    return this.http.get<Order>(`${this.baseUrl}/${id}`);
  }

  getAllowedStatuses(orderId: number): Observable<AllowedStatus[]> {
    return this.http.get<AllowedStatus[]>(
      `${this.baseUrl}/${orderId}/allowed-statuses`
    );
  }

  changeStatus(orderId: number, statusId: number, reason?: string): Observable<void> {
    const body = {
      statusId: statusId,
      reason: reason ?? null
    };

    return this.http.post<void>(
      `${this.baseUrl}/${orderId}/status`,
      body
    );
  }

  getOrderHistory(orderId: number): Observable<OrderStatusHistory[]> {
    return this.http.get<OrderStatusHistory[]>(
      `${this.baseUrl}/${orderId}/history`
    );
  }
}
