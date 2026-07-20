import { apiBaseUrl } from '../config/api-url';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PricingTier } from '../models/pricing-tier.model';

@Injectable({
  providedIn: 'root'
})
export class PricingService {

  private readonly apiUrl = `${apiBaseUrl}/pricing`;

  constructor(private http: HttpClient) { }

  getPricingTiers(): Observable<PricingTier[]> {
    return this.http.get<PricingTier[]>(`${this.apiUrl}/tiers`);
  }
}
