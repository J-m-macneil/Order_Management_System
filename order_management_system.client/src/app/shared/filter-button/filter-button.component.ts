import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-filter-button',
  standalone: false,
  templateUrl: './filter-button.component.html',
  styles: [`
    :host {
      display: block;
      min-width: 0;
    }

    button {
      width: 100%;
    }

    @media (min-width: 640px) {
      :host {
        flex: 0 0 auto;
      }

      button {
        width: auto;
      }
    }
  `]
})
export class FilterButtonComponent {
  @Input() activeFilterCount = 0;

  @Output() toggle = new EventEmitter<void>();
}
