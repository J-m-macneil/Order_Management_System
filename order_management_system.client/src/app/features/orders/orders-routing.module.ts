import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { OrderCreateComponent } from './order-create/order-create.component';
import { OrderDetailComponent } from './order-detail/order-detail.component';
import { OrdersComponent } from './orders.component';

const routes: Routes = [
  { path: '', component: OrdersComponent },
  { path: 'create', component: OrderCreateComponent },
  { path: ':id/edit', component: OrderCreateComponent },
  { path: ':id', component: OrderDetailComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OrdersRoutingModule { }
