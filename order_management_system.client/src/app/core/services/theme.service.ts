import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly storageKey = 'darkMode';

  isDarkMode = signal(false);

  initialize(): void {
    const savedDarkMode = localStorage.getItem(this.storageKey);
    const useDarkMode = savedDarkMode === null ? false : savedDarkMode === 'true';

    this.setDarkMode(useDarkMode);
  }

  toggleDarkMode(): void {
    this.setDarkMode(!this.isDarkMode());
  }

  setDarkMode(isDarkMode: boolean): void {
    this.isDarkMode.set(isDarkMode);

    if (isDarkMode) {
      document.documentElement.classList.add('dark');
    } else {
      document.documentElement.classList.remove('dark');
    }

    localStorage.setItem(this.storageKey, isDarkMode.toString());
  }
}
