import { TestBed } from '@angular/core/testing';
import { RouterModule } from '@angular/router';

import { ThemeService } from './core/services/theme.service';
import { AppComponent } from './app';

describe('AppComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AppComponent],
      imports: [RouterModule.forRoot([])],
      providers: [
        {
          provide: ThemeService,
          useValue: { initialize: () => undefined }
        }
      ]
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(AppComponent);

    expect(fixture.componentInstance).toBeTruthy();
  });
});
