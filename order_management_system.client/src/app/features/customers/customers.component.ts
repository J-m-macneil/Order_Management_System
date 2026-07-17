import { Component, OnInit, signal } from '@angular/core';
import { Customer } from '../../core/models/customer.model';
import { CustomersService } from '../../core/services/customers.service';
import { ToastService } from '../../core/services/toast.service';

@Component({
  selector: 'app-customers',
  standalone: false,
  templateUrl: './customers.component.html'
})
export class CustomersComponent implements OnInit {
  readonly customers = signal<Customer[]>([]);

  readonly isLoading = signal(false);
  readonly errorMessage = signal('');

  readonly pageNumber = signal(1);
  readonly pageSize = signal(25);
  readonly totalCount = signal(0);
  readonly totalPages = signal(0);
  readonly hasPreviousPage = signal(false);
  readonly hasNextPage = signal(false);

  searchTerm = '';
  industryFilter = '';
  paymentTermsFilter = '';
  activeFilter = '';

  filtersVisible = false;

  readonly stats = signal<{
    label: string;
    value: string | number;
    type: 'total' | 'active' | 'inactive';
    description: string;
  }[]>([]);
  readonly industries = signal<string[]>([]);
  readonly customerPendingDelete = signal<Customer | null>(null);

  constructor(
    private customersService: CustomersService,
    private toastService: ToastService
  ) { }

  ngOnInit(): void {
    this.loadSummary();
    this.loadIndustries();
    this.loadCustomers();
  }

  loadSummary(): void {
    this.customersService.getSummary().subscribe({
      next: (summary) => {
        this.stats.set([
          {
            label: 'Total Customers',
            value: summary.totalCustomers,
            type: 'total',
            description: 'All customer accounts'
          },
          {
            label: 'Active',
            value: summary.activeCustomers,
            type: 'active',
            description: 'Available for new orders'
          },
          {
            label: 'Inactive',
            value: summary.inactiveCustomers,
            type: 'inactive',
            description: 'Currently unavailable'
          }
        ]);
      }
    });
  }

  loadCustomers(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.customersService.getAll({
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      searchTerm: this.searchTerm.trim() || undefined,
      industryType: this.industryFilter || undefined,
      paymentTermsDays: this.paymentTermsFilter ? Number(this.paymentTermsFilter) : null,
      isActive: this.getActiveFilterValue()
    }).subscribe({
      next: (data) => {
        this.customers.set(data.items);
        this.pageNumber.set(data.pageNumber);
        this.pageSize.set(data.pageSize);
        this.totalCount.set(data.totalCount);
        this.totalPages.set(data.totalPages);
        this.hasPreviousPage.set(data.hasPreviousPage);
        this.hasNextPage.set(data.hasNextPage);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMessage.set('Failed to load customers.');
        this.isLoading.set(false);
      }
    });
  }

  toggleFilters(): void {
    this.filtersVisible = !this.filtersVisible;
  }

  applyFilters(): void {
    this.pageNumber.set(1);
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
        this.industries.set(industries);
      },
      error: () => {
        this.industries.set([]);
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
    this.customerPendingDelete.set(customer);
  }

  cancelDeleteCustomer(): void {
    this.customerPendingDelete.set(null);
  }

  confirmDeleteCustomer(): void {
    const customer = this.customerPendingDelete();
    if (!customer) {
      return;
    }

    const moveToPreviousPage = this.customers().length === 1 && this.pageNumber() > 1;

    this.customersService.delete(customer.customerId).subscribe({
      next: () => {
        this.customerPendingDelete.set(null);
        this.toastService.success('Customer deleted', `${customer.companyName} was removed.`);

        if (moveToPreviousPage) {
          this.pageNumber.update(page => page - 1);
        }

        this.loadSummary();
        this.loadCustomers();
      },
      error: () => {
        this.errorMessage.set('Failed to delete customer.');
        this.customerPendingDelete.set(null);
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

  onPageChange(pageNumber: number): void {
    this.pageNumber.set(pageNumber);
    this.loadCustomers();
  }

  onPageSizeChange(value: number): void {
    this.pageSize.set(value);
    this.pageNumber.set(1);
    this.loadCustomers();
  }
}
