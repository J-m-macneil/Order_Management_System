import { Component, HostListener } from '@angular/core';
import { ThemeService } from '../../core/services/theme.service';

@Component({
  selector: 'app-main-layout',
  standalone: false,
  templateUrl: './main-layout.component.html',
  styleUrls: ['./main-layout.component.css']
})
export class MainLayoutComponent {

  isNavOpen = false;

  constructor(public themeService: ThemeService) { }

  toggleNav() {
    this.isNavOpen = !this.isNavOpen;
  }

  closeNav() {
    this.isNavOpen = false;
  }

  @HostListener('window:orientationchange')
  onOrientationChange() {
    this.closeNav();
  }

}
