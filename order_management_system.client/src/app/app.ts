import { Component } from '@angular/core';
import { ThemeService } from './core/services/theme.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  standalone: false,
  styleUrls: ['./app.css']
})
export class AppComponent {
  title = 'Client';

  constructor(private themeService: ThemeService) {
    this.themeService.initialize();
  }
}
