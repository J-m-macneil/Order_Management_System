import { Component, EventEmitter, Input, OnDestroy, Output } from '@angular/core';

@Component({
  selector: 'app-search-input',
  standalone: false,
  templateUrl: './search-input.component.html',
  styleUrl: './search-input.component.css'
})
export class SearchInputComponent implements OnDestroy {
  @Input() value = '';
  @Input() placeholder = 'Search...';
  @Input() debounceMs = 200;

  @Output() valueChange = new EventEmitter<string>();
  @Output() search = new EventEmitter<void>();

  private searchDebounceTimeout?: ReturnType<typeof setTimeout>;

  onInput(value: string): void {
    this.value = value;
    this.valueChange.emit(value);

    clearTimeout(this.searchDebounceTimeout);

    this.searchDebounceTimeout = setTimeout(() => {
      this.search.emit();
    }, this.debounceMs);
  }

  ngOnDestroy(): void {
    clearTimeout(this.searchDebounceTimeout);
  }
}
