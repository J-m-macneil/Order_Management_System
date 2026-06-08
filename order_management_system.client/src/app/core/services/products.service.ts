import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

import { Product } from "../models/product.model";
import { ProductCategory } from "../models/product-category.model";
import { CreateProductRequest } from "../models/create-product.model";
import { UpdateProductRequest } from "../models/update-product.model";
import { UnitOfMeasure } from "../models/unit-of-measure.model";
import { HazardClass } from "../models/hazard-class.model";
import { ProductList } from "../models/product-list.model";
import { SafetyDataSheet, CreateSafetyDataSheetRequest, UpdateSafetyDataSheetRequest } from "../models/safety-data-sheet-model";
import { PaginationQuery } from "../models/pagination-query.model";
import { PagedResult } from "../models/paged-result.model";
import { ProductSummary } from "../models/product-summary.model";

@Injectable({
  providedIn: 'root'
})
export class ProductsService {
  private readonly baseUrl = 'https://localhost:7233/api/products';

  constructor(private http: HttpClient) { }

  getAll(query: PaginationQuery & {
    searchTerm?: string;
    isActive?: boolean | null;
    isRestricted?: boolean | null;
    isHazardous?: boolean | null;
    productCategoryId?: number | null;
    hazardClassId?: number | null;
  }): Observable<PagedResult<ProductList>> {
    let params = new HttpParams()
      .set('pageNumber', query.pageNumber)
      .set('pageSize', query.pageSize);

    if (query.searchTerm) {
      params = params.set('searchTerm', query.searchTerm);
    }

    if (query.isActive !== undefined && query.isActive !== null) {
      params = params.set('isActive', query.isActive);
    }

    if (query.isRestricted !== undefined && query.isRestricted !== null) {
      params = params.set('isRestricted', query.isRestricted);
    }

    if (query.isHazardous !== undefined && query.isHazardous !== null) {
      params = params.set('isHazardous', query.isHazardous);
    }

    if (query.productCategoryId) {
      params = params.set('productCategoryId', query.productCategoryId);
    }

    if (query.hazardClassId) {
      params = params.set('hazardClassId', query.hazardClassId);
    }

    return this.http.get<PagedResult<ProductList>>(this.baseUrl, { params });
  }

  getSummary(): Observable<ProductSummary> {
    return this.http.get<ProductSummary>(`${this.baseUrl}/summary`);
  }

  getById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateProductRequest): Observable<Product> {
    return this.http.post<Product>(this.baseUrl, request);
  }

  update(id: number, request: UpdateProductRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  getProductCategories(): Observable<ProductCategory[]> {
    return this.http.get<ProductCategory[]>('https://localhost:7233/api/product-categories');
  }

  getUnitsOfMeasure(): Observable<UnitOfMeasure[]> {
    return this.http.get<UnitOfMeasure[]>('https://localhost:7233/api/unit-of-measures');
  }

  getHazardClasses(): Observable<HazardClass[]> {
    return this.http.get<HazardClass[]>('https://localhost:7233/api/hazard-classes');
  }

  getSafetyDataSheets(productId: number): Observable<SafetyDataSheet[]> {
    return this.http.get<SafetyDataSheet[]>(`${this.baseUrl}/${productId}/sds`);
  }

  createSafetyDataSheet(productId: number, request: CreateSafetyDataSheetRequest): Observable<SafetyDataSheet> {
    return this.http.post<SafetyDataSheet>(`${this.baseUrl}/${productId}/sds`, request);
  }

  updateSafetyDataSheet(productId: number, sdsId: number, request: UpdateSafetyDataSheetRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${productId}/sds/${sdsId}`, request);
  }

  deleteSafetyDataSheet(productId: number, sdsId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${productId}/sds/${sdsId}`);
  }
}
