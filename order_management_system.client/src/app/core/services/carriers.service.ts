import { apiBaseUrl } from '../config/api-url';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Carrier } from '../models/carrier.model';

@Injectable({
  providedIn: 'root'
})
export class CarriersService {
  private readonly baseUrl = `${apiBaseUrl}/carriers`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<Carrier[]> {
    return this.http.get<Carrier[]>(this.baseUrl);
  }
}
