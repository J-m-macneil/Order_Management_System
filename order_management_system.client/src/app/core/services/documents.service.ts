import { apiBaseUrl } from '../config/api-url';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { OrderDocument } from '../models/order-document.model';

@Injectable({
  providedIn: 'root'
})
export class DocumentsService {
  private readonly apiUrl = `${apiBaseUrl}/documents`;

  constructor(private http: HttpClient) { }

  getDocumentsForOrder(orderId: number) {
    return this.http.get<OrderDocument[]>(`${this.apiUrl}/order/${orderId}`);
  }

  downloadDocument(documentId: number) {
    return this.http.get(`${this.apiUrl}/${documentId}/download`, {
      responseType: 'blob'
    });
  }
}
