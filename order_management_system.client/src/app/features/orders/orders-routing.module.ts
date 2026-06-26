import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { OrderCreateComponent } from './order-create/order-create.component';
import { OrderDetailComponent } from './order-detail/order-detail.component';
import { OrdersComponent } from './orders.component';

const routes: Routes = [
  { path: '', component: OrdersComponent, title: 'Orders | Back.' },
  { path: 'create', component: OrderCreateComponent, title: 'New Order | Back.' },
  { path: ':id/edit', component: OrderCreateComponent, title: 'Edit Order | Back.' },
  { path: ':id', component: OrderDetailComponent, title: 'Order Details | Back.' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OrdersRoutingModule { }
