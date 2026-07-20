import { apiBaseUrl } from '../config/api-url';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Warehouse } from '../models/warehouse-model';

@Injectable({
  providedIn: 'root'
})
export class WarehousesService {
  private readonly baseUrl = `${apiBaseUrl}/warehouses`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<Warehouse[]> {
    return this.http.get<Warehouse[]>(this.baseUrl);
  }
}
