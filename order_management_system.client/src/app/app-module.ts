import { HttpClientModule } from '@angular/common/http';
import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';

import { AppRoutingModule } from './app-routing-module';
import { AppComponent } from './app';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { NavbarComponent } from './layout/navbar/navbar.component';
import { SidebarComponent } from './layout/sidebar/sidebar.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { CustomersComponent } from './features/customers/customers.component';
import { ProductsComponent } from './features/products/products.component';
import { OrdersComponent } from './features/orders/orders.component';
import { AdminComponent } from './features/admin/admin.component';
import { LoginComponent } from './features/auth/login/login.component';
import { ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@NgModule({
  declarations: [
    AppComponent,
    MainLayoutComponent,
    NavbarComponent,
    SidebarComponent,
    DashboardComponent,
    CustomersComponent,
    ProductsComponent,
    OrdersComponent,
    AdminComponent,
    LoginComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    MatToolbarModule,
    MatListModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  providers: [provideBrowserGlobalErrorListeners()],
  bootstrap: [AppComponent],
})
export class AppModule { }
