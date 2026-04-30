import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-input-modal',
  templateUrl: './input-modal.component.html',
  standalone: false
})
export class InputModalComponent {
  @Input() title = 'Provide Input';
  @Input() placeholder = 'Enter value...';
  @Input() requireInput = true;

  value = '';

  @Output() confirm = new EventEmitter<string>();
  @Output() cancel = new EventEmitter<void>();

  submit(): void {
    if (this.requireInput && !this.value.trim()) return;
    this.confirm.emit(this.value.trim());
  }

  close(): void {
    this.cancel.emit();
  }
}
