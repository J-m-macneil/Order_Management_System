import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { LoginComponent } from './features/auth/login/login.component';

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
      {
        path: 'dashboard',
        canActivate: [AuthGuard],
        loadChildren: () => import('./features/dashboard/dashboard.module').then(m => m.DashboardModule)
      },

      {
        path: 'customers',
        canActivate: [AuthGuard],
        loadChildren: () => import('./features/customers/customers.module').then(m => m.CustomersModule)
      },

      {
        path: 'products',
        canActivate: [AuthGuard],
        loadChildren: () => import('./features/products/products.module').then(m => m.ProductsModule)
      },

      {
        path: 'orders',
        canActivate: [AuthGuard],
        loadChildren: () => import('./features/orders/orders.module').then(m => m.OrdersModule)
      },

      {
        path: 'admin',
        canActivate: [AuthGuard, AdminGuard],
        loadChildren: () => import('./features/admin/admin.module').then(m => m.AdminModule)
      }
    ]
  },
  { path: '**', redirectTo: 'login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
