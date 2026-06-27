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

  stats: { label: string; value: string | number }[] = [];
  industries: string[] = [];
  customerPendingDelete: Customer | null = null;

  constructor(
    private customersService: CustomersService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadSummary();
    this.loadIndustries();
    this.loadCustomers();
  }

  loadSummary(): void {
    this.customersService.getSummary().subscribe({
      next: (summary) => {
        this.stats = [
          {
            label: 'Total Customers',
            value: summary.totalCustomers
          },
          {
            label: 'Active',
            value: summary.activeCustomers
          },
          {
            label: 'Inactive',
            value: summary.inactiveCustomers
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

  loadIndustries(): void {
    this.customersService.getIndustryTypes().subscribe({
      next: (industries) => {
        this.industries = industries;
        this.cdr.detectChanges();
      },
      error: () => {
        this.industries = [];
        this.cdr.detectChanges();
      }
    });
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

  openDeleteCustomerModal(customer: Customer): void {
    this.customerPendingDelete = customer;
  }

  cancelDeleteCustomer(): void {
    this.customerPendingDelete = null;
  }

  confirmDeleteCustomer(): void {
    if (!this.customerPendingDelete) {
      return;
    }

    this.customersService.delete(this.customerPendingDelete.customerId).subscribe({
      next: () => {
        this.customerPendingDelete = null;
        this.loadSummary();
        this.loadCustomers();
      },
      error: () => {
        this.errorMessage = 'Failed to delete customer.';
        this.customerPendingDelete = null;
        this.cdr.detectChanges();
      }
    });
  }

  get activeFilterCount(): number {
    return [
      this.industryFilter,
      this.paymentTermsFilter,
      this.activeFilter
    ].filter(Boolean).length;
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
