import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DashboardMetricsResponse {
  metrics: {
    totalOrders: number;
    activeOrders: number;
    failedOrders: number;
    totalValue: number;
  };
  ordersByStatus: {
    status: string;
    count: number;
  }[];
  topCustomers: {
    name: string;
    orders: number;
  }[];
  recentFailures: {
    orderId: number;
    orderNumber: string;
    customer: string;
    reason: string;
    date: string;
  }[];
  priorityOrders: {
    orderId: number;
    orderNumber: string;
    customer: string;
    priority: string;
    dueDate: string;
  }[];
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly apiUrl = 'https://localhost:7233/api/dashboard';

  constructor(private http: HttpClient) { }

  getMetrics(): Observable<DashboardMetricsResponse> {
    return this.http.get<DashboardMetricsResponse>(`${this.apiUrl}/metrics`);
  }
}
