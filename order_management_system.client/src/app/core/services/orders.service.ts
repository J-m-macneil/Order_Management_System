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
  private readonly baseUrl = 'https://localhost:7233/api/orders';
    apiUrl: any;

  constructor(private http: HttpClient) { }

  getOrders(query: PaginationQuery): Observable<PagedResult<Order>> {
    const params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize);

    return this.http.get<PagedResult<Order>>(this.baseUrl, { params });
  }

  createOrder(order: CreateOrder): Observable<Order> {
    return this.http.post<Order>(this.baseUrl, order);
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
