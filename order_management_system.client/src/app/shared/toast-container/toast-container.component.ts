import { Component } from '@angular/core';

import { ToastNotification, ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-toast-container',
  standalone: false,
  templateUrl: './toast-container.component.html',
  styleUrls: ['./toast-container.component.css']
})
export class ToastContainerComponent {
  readonly notifications;

  constructor(private toastService: ToastService) {
    this.notifications = this.toastService.notifications;
  }

  dismiss(id: number): void {
    this.toastService.dismiss(id);
  }

  trackById(_: number, toast: ToastNotification): number {
    return toast.id;
  }
}
