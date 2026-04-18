import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

import { Product } from "../models/product.model";
import { ProductCategory } from "../models/product-category.model";
import { CreateProductRequest } from "../models/create-product.model";
import { UpdateProductRequest } from "../models/update-product.model";
import { UnitOfMeasure } from "../models/unit-of-measure.model";
import { HazardClass } from "../models/hazard-class.model";
import { ProductList } from "../models/product-list.model";

@Injectable({
  providedIn: 'root'
})
export class ProductsService {
  private readonly baseUrl = 'https://localhost:7233/api/products';

  constructor(private http: HttpClient) { }

  getAll(): Observable<ProductList[]> {
    return this.http.get<ProductList[]>(this.baseUrl);
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
}
