import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-table-pagination',
  standalone: false,
  templateUrl: './table-pagination.component.html',
  styleUrl: './table-pagination.component.css'
})
export class TablePaginationComponent {
  @Input() pageNumber = 1;
  @Input() pageSize = 25;
  @Input() totalCount = 0;
  @Input() totalPages = 0;
  @Input() hasPreviousPage = false;
  @Input() hasNextPage = false;
  @Input() pageSizeOptions: number[] = [25, 50, 100];
  @Input() itemLabel = 'items';

  @Output() pageSizeChange = new EventEmitter<number>();
  @Output() previousPage = new EventEmitter<void>();
  @Output() nextPage = new EventEmitter<void>();
}
