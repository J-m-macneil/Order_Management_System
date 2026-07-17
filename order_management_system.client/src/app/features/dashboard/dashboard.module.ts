import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import {
  LucideActivity,
  LucideClipboardList,
  LucideFlag,
  LucidePoundSterling,
  LucideTriangleAlert,
  LucideUsers
} from '@lucide/angular';

import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './dashboard.component';
import { OrderStatusOverviewComponent } from './order-status-overview/order-status-overview.component';

@NgModule({
  declarations: [
    DashboardComponent,
    OrderStatusOverviewComponent
  ],
  imports: [
    CommonModule,
    DashboardRoutingModule,
    LucideActivity,
    LucideClipboardList,
    LucideFlag,
    LucidePoundSterling,
    LucideTriangleAlert,
    LucideUsers
  ]
})
export class DashboardModule { }
