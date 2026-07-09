import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { SharedModule } from '../../shared/shared.module';
import { CustomerAddressesSectionComponent } from './customer-addresses-section/customer-addresses-section.component';
import { CustomerContactsSectionComponent } from './customer-contacts-section/customer-contacts-section.component';
import { CustomerFormComponent } from './customer-form/customer-form.component';
import { CustomersRoutingModule } from './customers-routing.module';
import { CustomersComponent } from './customers.component';

@NgModule({
  declarations: [
    CustomersComponent,
    CustomerFormComponent,
    CustomerAddressesSectionComponent,
    CustomerContactsSectionComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    CustomersRoutingModule
  ]
})
export class CustomersModule { }
