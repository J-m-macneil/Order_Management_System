import { Component, signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { RouterModule } from '@angular/router';

import { ThemeService } from '../../core/services/theme.service';
import { MainLayoutComponent } from './main-layout.component';

@Component({
  selector: 'app-navbar',
  standalone: false,
  template: ''
})
class NavbarStubComponent { }

@Component({
  selector: 'app-toast-container',
  standalone: false,
  template: ''
})
class ToastContainerStubComponent { }

describe('MainLayoutComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MainLayoutComponent, NavbarStubComponent, ToastContainerStubComponent],
      imports: [RouterModule.forRoot([])],
      providers: [
        {
          provide: ThemeService,
          useValue: { isDarkMode: signal(true) }
        }
      ]
    }).compileComponents();
  });

  it('should create', () => {
    const fixture = TestBed.createComponent(MainLayoutComponent);

    expect(fixture.componentInstance).toBeTruthy();
  });
});
