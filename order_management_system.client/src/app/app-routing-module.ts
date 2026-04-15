import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { CustomersComponent } from './features/customers/customers.component';
import { ProductsComponent } from './features/products/products.component';
import { OrdersComponent } from './features/orders/orders.component';
import { AuthGuard } from './core/guards/auth.guard';
import { AdminGuard } from './core/guards/admin.guard';
import { AdminComponent } from './features/admin/admin.component';
import { LoginComponent } from './features/auth/login/login.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'customers', component: CustomersComponent, canActivate: [AuthGuard] },
      { path: 'products', component: ProductsComponent, canActivate: [AuthGuard] },
      { path: 'orders', component: OrdersComponent, canActivate: [AuthGuard] },
      { path: 'admin', component: AdminComponent, canActivate: [AuthGuard, AdminGuard] }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
