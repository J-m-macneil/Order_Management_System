import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { InputModalComponent } from '../input-modal/input-modal.component';
import { ProcessingJobsPanelComponent } from '../processing-jobs-panel/processing-jobs-panel.component';
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
    ProcessingJobsPanelComponent,
    InputModalComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    OrdersRoutingModule
  ]
})
export class OrdersModule { }
