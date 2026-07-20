import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ProcessingJobsPanelComponent } from '../processing-jobs-panel/processing-jobs-panel.component';
import { SharedModule } from '../../shared/shared.module';
import { OrderActivityCardComponent } from './order-activity-card/order-activity-card.component';
import { OrderCreateComponent } from './order-create/order-create.component';
import { OrderDetailComponent } from './order-detail/order-detail.component';
import { OrderDocumentsPanelComponent } from './order-documents-panel/order-documents-panel.component';
import { OrderInformationCardComponent } from './order-information-card/order-information-card.component';
import { OrderItemsEditorComponent } from './order-items-editor/order-items-editor.component';
import { OrderItemsTableComponent } from './order-items-table/order-items-table.component';
import { OrderLogisticsCardComponent } from './order-logistics-card/order-logistics-card.component';
import { OrderNotesCardComponent } from './order-notes-card/order-notes-card.component';
import { OrderStatusActionsComponent } from './order-status-actions/order-status-actions.component';
import { OrderStatusHistoryComponent } from './order-status-history/order-status-history.component';
import { OrderStatusStripComponent } from './order-status-strip/order-status-strip.component';
import { OrderSummaryCardComponent } from './order-summary-card/order-summary-card.component';
import { OrdersRoutingModule } from './orders-routing.module';
import { OrdersComponent } from './orders.component';

@NgModule({
  declarations: [
    OrdersComponent,
    OrderActivityCardComponent,
    OrderCreateComponent,
    OrderDetailComponent,
    OrderInformationCardComponent,
    OrderItemsEditorComponent,
    OrderItemsTableComponent,
    OrderLogisticsCardComponent,
    OrderNotesCardComponent,
    OrderStatusActionsComponent,
    OrderStatusHistoryComponent,
    OrderStatusStripComponent,
    OrderSummaryCardComponent,
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
