import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-table-pagination',
  standalone: false,
  templateUrl: './table-pagination.component.html'
})
export class TablePaginationComponent {
  @Input() pageNumber = 1;
  @Input() pageSize = 25;
  @Input() totalCount = 0;
  @Input() totalPages = 0;
  @Input() hasPreviousPage = false;
  @Input() hasNextPage = false;
  @Input() itemLabel = 'items';

  readonly pageSizeOptions = [25, 50, 100];

  @Output() pageSizeChange = new EventEmitter<number>();
  @Output() pageChange = new EventEmitter<number>();
}
