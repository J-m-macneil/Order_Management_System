import { HttpClient } from '@angular/common/http';
import { Component, signal } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  standalone: false,
  styleUrl: './app.css'
})
export class App {
  ngOnInit() {
      throw new Error('Method not implemented.');
  }

  constructor(private http: HttpClient) {}

  protected readonly title = signal('order_management_system.client');
}

