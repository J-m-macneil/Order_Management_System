import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LucidePlus } from '@lucide/angular';

import { ConfirmationModalComponent } from './confirmation-modal/confirmation-modal.component';
import { FilterButtonComponent } from './filter-button/filter-button.component';
import { SearchInputComponent } from './search-input/search-input.component';
import { TablePaginationComponent } from './table-pagination/table-pagination.component';

@NgModule({
  declarations: [
    ConfirmationModalComponent,
    SearchInputComponent,
    FilterButtonComponent,
    TablePaginationComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    LucidePlus
  ],
  exports: [
    ConfirmationModalComponent,
    SearchInputComponent,
    FilterButtonComponent,
    TablePaginationComponent,
    LucidePlus
  ]
})
export class SharedModule { }
