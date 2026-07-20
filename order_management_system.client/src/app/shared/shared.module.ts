import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { LucidePlus } from '@lucide/angular';

import { ConfirmationModalComponent } from './confirmation-modal/confirmation-modal.component';
import { FilterButtonComponent } from './filter-button/filter-button.component';
import { SearchInputComponent } from './search-input/search-input.component';
import { TablePaginationComponent } from './table-pagination/table-pagination.component';
import { DemoWriteDirective } from './demo-write/demo-write.directive';

@NgModule({
  declarations: [
    ConfirmationModalComponent,
    SearchInputComponent,
    FilterButtonComponent,
    TablePaginationComponent,
    DemoWriteDirective
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
    DemoWriteDirective,
    LucidePlus
  ]
})
export class SharedModule { }
