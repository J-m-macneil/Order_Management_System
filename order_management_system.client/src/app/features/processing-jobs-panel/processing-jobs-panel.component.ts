import { Component, Input, OnChanges, SimpleChanges, ChangeDetectorRef, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../../core/auth/auth.service';
import { OrderStatus } from '../../core/models/order-status.enum';
import { ProcessingJob } from '../../core/models/processing-job.model';
import { ProcessingJobsService } from '../../core/services/processing-jobs.service';
import { getApiErrorMessage } from '../../core/utils/api-error-message';

type ProcessingJobDisplay = Partial<ProcessingJob> & {
  jobType: string;
  status: string;
  isPlaceholder: boolean;
};

@Component({
  selector: 'app-processing-jobs-panel',
  standalone: false,
  templateUrl: './processing-jobs-panel.component.html',
  styleUrls: ['./processing-jobs-panel.component.css']
})
export class ProcessingJobsPanelComponent implements OnChanges {
  @Input() orderId!: number;
  @Input('orderStatusId') orderStatusId: number | null = null;
  @Input() requiresSdsBundle = false;
  @Output() jobsChanged = new EventEmitter<void>();

  processingJobs: ProcessingJob[] = [];
  displayJobs: ProcessingJobDisplay[] = [];
  isLoading = false;
  isRetryingJobId: number | null = null;
  errorMessage = '';

  private readonly ukDateFormatter = new Intl.DateTimeFormat('en-GB', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
    hour12: false,
    timeZone: 'Europe/London'
  });

  constructor(
    private processingJobsService: ProcessingJobsService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    if ((changes['orderId'] || changes['orderStatusId'] || changes['requiresSdsBundle']) && this.orderId) {
      this.loadProcessingJobs();
    }
  }

  loadProcessingJobs(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.processingJobsService.getJobsForOrder(this.orderId).subscribe({
      next: (jobs: ProcessingJob[]) => {
        this.processingJobs = jobs;
        this.displayJobs = this.buildDisplayJobs(this.processingJobs);
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.processingJobs = [];
        this.displayJobs = [];
        this.errorMessage = 'Failed to load processing jobs.';
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }

  canRetryJob(job: ProcessingJobDisplay): boolean {
    return !job.isPlaceholder &&
      !!job.processingJobId &&
      this.isFailedJob(job) &&
      this.authService.hasRole('Operations', 'Admin');
  }

  isFailedJob(job: ProcessingJobDisplay): boolean {
    return job.status.trim().toLowerCase() === 'failed';
  }

  shouldShowMissingJobsWarning(): boolean {
    return !this.isLoading &&
      !this.errorMessage &&
      this.processingJobs.length === 0 &&
      (this.orderStatusId === OrderStatus.Approved || this.orderStatusId === OrderStatus.InProcessing);
  }

  retryJob(job: ProcessingJobDisplay): void {
    if (!this.canRetryJob(job) || this.isRetryingJobId || !job.processingJobId) {
      return;
    }

    this.isRetryingJobId = job.processingJobId;
    this.errorMessage = '';

    this.processingJobsService.retryJob(job.processingJobId).subscribe({
      next: () => {
        this.isRetryingJobId = null;
        this.jobsChanged.emit();
        this.loadProcessingJobs();
      },
      error: (err) => {
        this.errorMessage =
          getApiErrorMessage(err, 'Failed to retry processing job.');
        this.isRetryingJobId = null;
        this.cdr.markForCheck();
      }
    });
  }

  getJobStatusClass(status: string): string {
    switch (status.trim().toLowerCase()) {
      case 'completed':
        return 'app-badge app-badge--success';
      case 'processing':
        return 'app-badge app-badge--info';
      case 'queued':
      case 'pending':
        return 'app-badge app-badge--warning';
      case 'failed':
        return 'app-badge app-badge--danger';
      default:
        return 'app-badge app-badge--neutral';
    }
  }

  formatUkDate(value?: string | null): string {
    if (!value) {
      return '-';
    }

    const hasTimeZone = /(?:z|[+-]\d{2}:?\d{2})$/i.test(value);
    const date = new Date(hasTimeZone ? value : `${value}Z`);

    return Number.isNaN(date.getTime())
      ? '-'
      : this.ukDateFormatter.format(date).replace(',', '');
  }

  private buildDisplayJobs(jobs: ProcessingJob[]): ProcessingJobDisplay[] {
    if (!this.shouldShowWorkflowSteps(jobs)) {
      return jobs.map(job => ({ ...job, isPlaceholder: false }));
    }

    const expectedJobTypes = this.getExpectedWorkflowJobTypes();
    const expectedJobTypeSet = new Set(expectedJobTypes);
    const jobsByType = new Map(jobs.map(job => [job.jobType, job]));

    const expectedJobs = expectedJobTypes.map(jobType => {
      const job = jobsByType.get(jobType);

      return job
        ? { ...job, isPlaceholder: false }
        : this.createPlaceholderJob(jobType);
    });

    const extraJobs = jobs
      .filter(job => !expectedJobTypeSet.has(job.jobType))
      .map(job => ({ ...job, isPlaceholder: false }));

    return [...expectedJobs, ...extraJobs];
  }

  private shouldShowWorkflowSteps(jobs: ProcessingJob[]): boolean {
    return jobs.length > 0 ||
      this.orderStatusId === OrderStatus.Approved ||
      this.orderStatusId === OrderStatus.InProcessing ||
      this.orderStatusId === OrderStatus.AwaitingDispatch ||
      this.orderStatusId === OrderStatus.Failed;
  }

  private getExpectedWorkflowJobTypes(): string[] {
    return [
      'PushToLogisticsProvider',
      'GenerateSdsBundle',
      'GenerateOrderSummaryDocument'
    ];
  }

  private createPlaceholderJob(jobType: string): ProcessingJobDisplay {
    return {
      jobType,
      status: jobType === 'GenerateSdsBundle' && !this.requiresSdsBundle ? 'Not Required' : 'Pending',
      isPlaceholder: true
    };
  }
}
