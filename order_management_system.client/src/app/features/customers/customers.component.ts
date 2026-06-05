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

  private filtersVisible = false;

  stats: { label: string; value: string | number; color: string }[] = [];
  industries: string[] = [];

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

    this.customersService.getAll({
      pageNumber: this.pageNumber,
      pageSize: this.pageSize
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
    const term = this.searchTerm.toLowerCase().trim();

    this.filteredCustomers = this.customers.filter(customer => {
      const matchesSearch =
        !term ||
        customer.accountNumber?.toLowerCase().includes(term) ||
        customer.companyName?.toLowerCase().includes(term) ||
        customer.industryType?.toLowerCase().includes(term) ||
        customer.mainContactName?.toLowerCase().includes(term) ||
        customer.mainContactEmail?.toLowerCase().includes(term);

      const matchesIndustry =
        !this.industryFilter ||
        customer.industryType === this.industryFilter;

      const matchesPaymentTerms =
        !this.paymentTermsFilter ||
        customer.paymentTermsDays?.toString() === this.paymentTermsFilter;

      return matchesSearch && matchesIndustry && matchesPaymentTerms;
    });
  }

  private initialiseCustomerDashboard(): void {
    this.industries = Array.from(
      new Set(
        this.customers
          .map(customer => customer.industryType)
          .filter((industry): industry is string => !!industry)
      )
    );

    this.updateStats();
    this.applyFilters();
  }

  private updateStats(): void {
    const activeCustomers = this.customers.filter(customer => customer.isActive).length;
    const inactiveCustomers = this.customers.filter(customer => !customer.isActive).length;

    this.stats = [
      {
        label: 'Total Customers',
        value: this.customers.length,
        color: ''
      },
      {
        label: 'Active',
        value: activeCustomers,
        color: 'text-emerald-600 dark:text-emerald-400'
      },
      {
        label: 'Inactive',
        value: inactiveCustomers,
        color: 'text-red-600 dark:text-red-400'
      }
    ];
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
