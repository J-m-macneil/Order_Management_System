import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CustomerFormComponent } from './customer-form/customer-form.component';
import { CustomersComponent } from './customers.component';

const routes: Routes = [
  { path: '', component: CustomersComponent, title: 'Customers | Back.' },
  { path: 'new', component: CustomerFormComponent, title: 'New Customer | Back.' },
  { path: 'edit/:id', component: CustomerFormComponent, title: 'Edit Customer | Back.' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CustomersRoutingModule { }
