import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CustomersService } from './customers.service';
import { Customer } from '../models/customer.model';

@Component({
  selector: 'app-customers',
  standalone: false,
  templateUrl: './customers.component.html',
  styleUrls: ['./customers.component.css']
})
export class CustomersComponent implements OnInit {
  customers: Customer[] = [];
  isLoading = false;
  errorMessage = '';

  constructor(
    private customersService: CustomersService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadCustomers();
  }

  loadCustomers(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.customersService.getAll().subscribe({
      next: (data) => {
        this.customers = data;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Failed to load customers.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  deleteCustomer(id: number): void {
    if (!confirm('Are you sure you want to delete this customer?')) {
      return;
    }

    this.customersService.delete(id).subscribe({
      next: () => this.loadCustomers(),
      error: () => {
        this.errorMessage = 'Failed to delete customer.';
        this.cdr.detectChanges();
      }
    });
  }
}
