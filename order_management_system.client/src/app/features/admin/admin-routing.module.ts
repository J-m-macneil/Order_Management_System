import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AuditLogsComponent } from '../audit-logs/audit-logs.component';
import { AdminComponent } from './admin.component';

const routes: Routes = [
  { path: '', component: AdminComponent },
  { path: 'audit', component: AuditLogsComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
