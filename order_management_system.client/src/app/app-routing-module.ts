import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { CustomersComponent } from './features/customers/customers.component';
import { CustomerFormComponent } from './features/customers/customer-form/customer-form.component';
import { ProductsComponent } from './features/products/products.component';
import { ProductFormComponent } from './features/products/product-form/product-form.component';
import { OrdersComponent } from './features/orders/orders.component';
import { AdminComponent } from './features/admin/admin.component';
import { LoginComponent } from './features/auth/login/login.component';
import { OrderCreateComponent } from './features/orders/order-create/order-create.component';
import { OrderDetailComponent } from './features/orders/order-detail/order-detail.component';
import { AuditLogsComponent } from './features/audit-logs/audit-logs.component';

import { AuthGuard } from './core/guards/auth.guard';
import { AdminGuard } from './core/guards/admin.guard';
import { LoginGuard } from './core/guards/login.guard';

const routes: Routes = [
  { path: '', component: LoginComponent, canActivate: [LoginGuard], pathMatch: 'full' },
  { path: 'login', component: LoginComponent, canActivate: [LoginGuard] },
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },

      { path: 'customers', component: CustomersComponent, canActivate: [AuthGuard] },
      { path: 'customers/new', component: CustomerFormComponent, canActivate: [AuthGuard] },
      { path: 'customers/edit/:id', component: CustomerFormComponent, canActivate: [AuthGuard] },

      { path: 'products', component: ProductsComponent, canActivate: [AuthGuard] },
      { path: 'products/create', component: ProductFormComponent, canActivate: [AuthGuard] },
      { path: 'products/edit/:id', component: ProductFormComponent, canActivate: [AuthGuard] },

      { path: 'orders', component: OrdersComponent, canActivate: [AuthGuard] },
      { path: 'admin', component: AdminComponent, canActivate: [AuthGuard, AdminGuard] },
      { path: 'admin/audit', component: AuditLogsComponent, canActivate: [AuthGuard, AdminGuard] },

      { path: 'orders/create', component: OrderCreateComponent, canActivate: [AuthGuard] },
      { path: 'orders/:id/edit', component: OrderCreateComponent, canActivate: [AuthGuard] },
      { path: 'orders/:id', component: OrderDetailComponent, canActivate: [AuthGuard] }
    ]
  },
  { path: '**', redirectTo: 'login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
