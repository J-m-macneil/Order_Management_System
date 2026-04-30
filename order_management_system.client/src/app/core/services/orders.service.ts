import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateOrder } from '../models/create-order.model';
import { Order } from '../models/order.model';
import { AllowedStatus } from '../models/allowed-status.model';
import { ChangeOrderStatusRequest } from '../models/change-order-status-request.model';
import { OrderStatus } from '../models/order-status.enum';
import { OrderStatusHistory } from '../models/order-status-history.model';
import { ProcessingJob } from '../models/processing-job.model';

@Injectable({
  providedIn: 'root'
})
export class OrdersService {
  private readonly baseUrl = 'https://localhost:7233/api/orders';
    apiUrl: any;

  constructor(private http: HttpClient) { }

  getOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(this.baseUrl);
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

  changeStatus(
    orderId: number,
    status: OrderStatus,
    reason?: string
  ): Observable<void> {
    const request: ChangeOrderStatusRequest = {
      status,
      reason
    };

    return this.http.post<void>(
      `${this.baseUrl}/${orderId}/status`,
      request
    );
  }

  getOrderHistory(orderId: number): Observable<OrderStatusHistory[]> {
    return this.http.get<OrderStatusHistory[]>(
      `${this.baseUrl}/${orderId}/history`
    );
  }
}
