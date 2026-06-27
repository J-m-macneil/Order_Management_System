import { Component, EventEmitter, Input, Output } from '@angular/core';

export type ConfirmationModalVariant = 'default' | 'warning' | 'danger';

@Component({
  selector: 'app-confirmation-modal',
  standalone: false,
  templateUrl: './confirmation-modal.component.html'
})
export class ConfirmationModalComponent {
  @Input() title = 'Confirm Action';
  @Input() message = '';
  @Input() confirmText = 'Confirm';
  @Input() cancelText = 'Cancel';
  @Input() variant: ConfirmationModalVariant = 'default';
  @Input() requireReason = false;
  @Input() reasonLabel = 'Reason';
  @Input() reasonPlaceholder = 'Enter reason...';

  @Output() confirm = new EventEmitter<string | undefined>();
  @Output() cancel = new EventEmitter<void>();

  reason = '';

  get confirmButtonClass(): string {
    return this.variant === 'danger'
      ? 'app-danger-button px-4 py-2 text-sm'
      : 'app-primary-button px-4 py-2 text-sm';
  }

  submit(): void {
    const trimmedReason = this.reason.trim();

    if (this.requireReason && !trimmedReason) {
      return;
    }

    this.confirm.emit(trimmedReason || undefined);
  }

  close(): void {
    this.cancel.emit();
  }
}
