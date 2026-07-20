import { Directive, HostBinding, HostListener } from '@angular/core';

import { AuthService } from '../../core/auth/auth.service';

@Directive({
  selector: '[appDemoWrite]',
  standalone: false
})
export class DemoWriteDirective {
  constructor(private authService: AuthService) { }

  @HostBinding('class.app-demo-write-disabled')
  get isDisabled(): boolean {
    return this.authService.isDemoUser();
  }

  @HostBinding('attr.aria-disabled')
  get ariaDisabled(): 'true' | null {
    return this.isDisabled ? 'true' : null;
  }

  @HostBinding('attr.title')
  get title(): string | null {
    return this.isDisabled ? 'Demo access is read-only' : null;
  }

  @HostListener('click', ['$event'])
  blockClick(event: Event): void {
    if (!this.isDisabled) {
      return;
    }

    event.preventDefault();
    event.stopImmediatePropagation();
  }
}
