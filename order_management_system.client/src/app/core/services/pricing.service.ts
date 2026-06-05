import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PricingTier } from '../models/pricing-tier.model';

@Injectable({
  providedIn: 'root'
})
export class PricingService {

  private readonly apiUrl = 'https://localhost:7233/api/pricing';

  constructor(private http: HttpClient) { }

  getPricingTiers(): Observable<PricingTier[]> {
    return this.http.get<PricingTier[]>(`${this.apiUrl}/tiers`);
  }
}
