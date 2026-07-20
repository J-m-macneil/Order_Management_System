import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LucidePackage, LucidePackageCheck, LucideShieldAlert, LucideTriangleAlert } from '@lucide/angular';

import { SharedModule } from '../../shared/shared.module';
import { ProductAuditPanelComponent } from './product-audit-panel/product-audit-panel.component';
import { ProductFormComponent } from './product-form/product-form.component';
import { ProductSdsPanelComponent } from './product-sds-panel/product-sds-panel.component';
import { ProductsRoutingModule } from './products-routing.module';
import { ProductsComponent } from './products.component';

@NgModule({
  declarations: [
    ProductsComponent,
    ProductFormComponent,
    ProductSdsPanelComponent,
    ProductAuditPanelComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    LucidePackage,
    LucidePackageCheck,
    LucideShieldAlert,
    LucideTriangleAlert,
    SharedModule,
    ProductsRoutingModule
  ]
})
export class ProductsModule { }
