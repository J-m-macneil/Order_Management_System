import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ProcessingJobsPanelComponent } from '../processing-jobs-panel/processing-jobs-panel.component';
import { SharedModule } from '../../shared/shared.module';
import { OrderCreateComponent } from './order-create/order-create.component';
import { OrderDetailComponent } from './order-detail/order-detail.component';
import { OrderDocumentsPanelComponent } from './order-documents-panel/order-documents-panel.component';
import { OrdersRoutingModule } from './orders-routing.module';
import { OrdersComponent } from './orders.component';

@NgModule({
  declarations: [
    OrdersComponent,
    OrderCreateComponent,
    OrderDetailComponent,
    OrderDocumentsPanelComponent,
    ProcessingJobsPanelComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    OrdersRoutingModule
  ]
})
export class OrdersModule { }
