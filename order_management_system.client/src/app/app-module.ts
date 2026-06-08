import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing-module';
import { AppComponent } from './app';

import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { NavbarComponent } from './layout/navbar/navbar.component';
import { SidebarComponent } from './layout/sidebar/sidebar.component';

import { DashboardComponent } from './features/dashboard/dashboard.component';
import { CustomersComponent } from './features/customers/customers.component';
import { CustomerFormComponent } from './features/customers/customer-form/customer-form.component';
import { ProductsComponent } from './features/products/products.component';
import { ProductFormComponent } from './features/products/product-form/product-form.component';
import { OrdersComponent } from './features/orders/orders.component';
import { AdminComponent } from './features/admin/admin.component';
import { LoginComponent } from './features/auth/login/login.component';
import { AuthInterceptor } from './core/interceptors/auth.interceptor';
import { OrderCreateComponent } from './features/orders/order-create/order-create.component';
import { OrderDetailComponent } from './features/orders/order-detail/order-detail.component';
import { InputModalComponent } from './features/input-modal/input-modal.component';
import { ProcessingJobsPanelComponent } from './features/processing-jobs-panel/processing-jobs-panel.component';
import { OrderDocumentsPanelComponent } from './features/orders/order-documents-panel/order-documents-panel.component';
import { AuditLogsComponent } from './features/audit-logs/audit-logs.component';

@NgModule({
  declarations: [
    AppComponent,
    MainLayoutComponent,
    NavbarComponent,
    SidebarComponent,
    DashboardComponent,
    CustomersComponent,
    CustomerFormComponent,
    ProductsComponent,
    ProductFormComponent,
    OrdersComponent,
    OrderCreateComponent,
    OrderDetailComponent,
    AdminComponent,
    LoginComponent,
    InputModalComponent,
    ProcessingJobsPanelComponent,
    OrderDocumentsPanelComponent,
    AuditLogsComponent
  ],
  imports: [
    BrowserModule,
    CommonModule,
    HttpClientModule,
    AppRoutingModule,
    ReactiveFormsModule,
    FormsModule
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
