export type ConfirmationModalVariant = 'default' | 'warning' | 'danger';
export type PendingConfirmationAction = 'status' | 'discardDraft';

export interface ConfirmationModalState {
  isOpen: boolean;
  title: string;
  message: string;
  confirmText: string;
  variant: ConfirmationModalVariant;
  requireReason: boolean;
  reasonPlaceholder: string;
}
