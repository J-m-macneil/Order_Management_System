import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Customer } from '../../core/models/customer.model';
import { CustomersService } from '../../core/services/customers.service';

@Component({
  selector: 'app-customers',
  standalone: false,
  templateUrl: './customers.component.html',
  styleUrls: ['./customers.component.css']
})
export class CustomersComponent implements OnInit {
  customers: Customer[] = [];
  filteredCustomers: Customer[] = [];

  isLoading = false;
  errorMessage = '';

  pageNumber = 1;
  pageSize = 25;
  totalCount = 0;
  totalPages = 0;
  hasPreviousPage = false;
  hasNextPage = false;
  pageSizeOptions = [25, 50, 100];

  searchTerm = '';
  industryFilter = '';
  paymentTermsFilter = '';
  activeFilter = '';

  private filtersVisible = false;

  stats: { label: string; value: string | number; color: string }[] = [];
  industries: string[] = [];

  constructor(
    private customersService: CustomersService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadSummary();
    this.loadCustomers();
  }

  loadSummary(): void {
    this.customersService.getSummary().subscribe({
      next: (summary) => {
        this.stats = [
          {
            label: 'Total Customers',
            value: summary.totalCustomers,
            color: ''
          },
          {
            label: 'Active',
            value: summary.activeCustomers,
            color: 'text-emerald-600 dark:text-emerald-400'
          },
          {
            label: 'Inactive',
            value: summary.inactiveCustomers,
            color: 'text-red-600 dark:text-red-400'
          }
        ];
        this.cdr.detectChanges();
      }
    });
  }

  loadCustomers(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.customersService.getAll({
      pageNumber: this.pageNumber,
      pageSize: this.pageSize,
      searchTerm: this.searchTerm.trim() || undefined,
      industryType: this.industryFilter || undefined,
      paymentTermsDays: this.paymentTermsFilter ? Number(this.paymentTermsFilter) : null,
      isActive: this.getActiveFilterValue()
    }).subscribe({
      next: (data) => {
        this.customers = data.items;
        this.pageNumber = data.pageNumber;
        this.pageSize = data.pageSize;
        this.totalCount = data.totalCount;
        this.totalPages = data.totalPages;
        this.hasPreviousPage = data.hasPreviousPage;
        this.hasNextPage = data.hasNextPage;
        this.initialiseCustomerDashboard();
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

  showFilters(): boolean {
    return this.filtersVisible;
  }

  toggleFilters(): void {
    this.filtersVisible = !this.filtersVisible;
  }

  applyFilters(): void {
    this.pageNumber = 1;
    this.loadCustomers();
  }

  clearFilters(): void {
    this.industryFilter = '';
    this.paymentTermsFilter = '';
    this.activeFilter = '';
    this.applyFilters();
  }

  private initialiseCustomerDashboard(): void {
    this.filteredCustomers = this.customers;

    this.industries = Array.from(
      new Set(
        this.customers
          .map(customer => customer.industryType)
          .filter((industry): industry is string => !!industry)
      )
    );
  }

  private getActiveFilterValue(): boolean | null {
    if (this.activeFilter === 'active') {
      return true;
    }

    if (this.activeFilter === 'inactive') {
      return false;
    }

    return null;
  }

  deleteCustomer(id: number): void {
    if (!confirm('Are you sure you want to delete this customer?')) {
      return;
    }

    this.customersService.delete(id).subscribe({
      next: () => {
        this.loadSummary();
        this.loadCustomers();
      },
      error: () => {
        this.errorMessage = 'Failed to delete customer.';
        this.cdr.detectChanges();
      }
    });
  }

  goToPreviousPage(): void {
    if (!this.hasPreviousPage) {
      return;
    }

    this.pageNumber--;
    this.loadCustomers();
  }

  goToNextPage(): void {
    if (!this.hasNextPage) {
      return;
    }

    this.pageNumber++;
    this.loadCustomers();
  }

  onPageSizeChange(value: number): void {
    if (!this.pageSizeOptions.includes(value)) {
      return;
    }

    this.pageSize = value;
    this.pageNumber = 1;
    this.loadCustomers();
  }
}
