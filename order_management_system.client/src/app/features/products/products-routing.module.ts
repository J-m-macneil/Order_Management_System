import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ProductFormComponent } from './product-form/product-form.component';
import { ProductsComponent } from './products.component';

const routes: Routes = [
  { path: '', component: ProductsComponent, title: 'Products | Back.' },
  { path: 'create', component: ProductFormComponent, title: 'New Product | Back.' },
  { path: 'edit/:id', component: ProductFormComponent, title: 'Edit Product | Back.' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProductsRoutingModule { }
