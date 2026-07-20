import { Injectable, signal } from '@angular/core';

export type ToastType = 'success' | 'error' | 'warning' | 'info';

export interface ToastNotification {
  id: number;
  type: ToastType;
  title: string;
  message?: string;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private nextId = 1;
  private readonly activeToasts = signal<ToastNotification[]>([]);

  readonly notifications = this.activeToasts.asReadonly();

  success(title: string, message?: string): void {
    this.show('success', title, message);
  }

  error(title: string, message?: string): void {
    this.show('error', title, message, 8000);
  }

  warning(title: string, message?: string): void {
    this.show('warning', title, message, 7000);
  }

  info(title: string, message?: string): void {
    this.show('info', title, message);
  }

  dismiss(id: number): void {
    this.activeToasts.update(toasts => toasts.filter(toast => toast.id !== id));
  }

  private show(
    type: ToastType,
    title: string,
    message?: string,
    duration = 5000
  ): void {
    const notification: ToastNotification = {
      id: this.nextId++,
      type,
      title,
      message
    };

    this.activeToasts.update(toasts => [...toasts.slice(-2), notification]);
    window.setTimeout(() => this.dismiss(notification.id), duration);
  }
}
