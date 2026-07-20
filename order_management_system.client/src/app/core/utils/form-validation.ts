import { AbstractControl } from '@angular/forms';

export const PHONE_NUMBER_PATTERN = /^[0-9+\s()\-]*$/;

export function getValidationMessage(control: AbstractControl | null, label: string): string | null {
  if (!control?.touched || !control.errors) {
    return null;
  }

  if (control.hasError('required')) {
    return `${label} is required.`;
  }

  if (control.hasError('email')) {
    return 'Enter a valid email address.';
  }

  if (control.hasError('min')) {
    return `${label} cannot be negative.`;
  }

  if (control.hasError('max')) {
    return `${label} is too high.`;
  }

  const maxLengthError = control.getError('maxlength');
  if (maxLengthError) {
    return `${label} must be ${maxLengthError.requiredLength} characters or fewer.`;
  }

  const minLengthError = control.getError('minlength');
  if (minLengthError) {
    return `${label} must be at least ${minLengthError.requiredLength} characters.`;
  }

  if (control.hasError('pattern')) {
    return `Enter a valid ${label.toLowerCase()}.`;
  }

  return `Enter a valid ${label.toLowerCase()}.`;
}
