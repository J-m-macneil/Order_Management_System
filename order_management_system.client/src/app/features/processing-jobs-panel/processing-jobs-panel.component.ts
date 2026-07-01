import { Component, Input, OnChanges, SimpleChanges, ChangeDetectorRef } from '@angular/core';
import { AuthService } from '../../core/auth/auth.service';
import { ProcessingJob } from '../../core/models/processing-job.model';
import { ProcessingJobsService } from '../../core/services/processing-jobs.service';

@Component({
  selector: 'app-processing-jobs-panel',
  standalone: false,
  templateUrl: './processing-jobs-panel.component.html',
  styleUrls: ['./processing-jobs-panel.component.css']
})
export class ProcessingJobsPanelComponent implements OnChanges {
  @Input() orderId!: number;
  @Input('orderStatusId') orderStatusId: number | null = null;

  processingJobs: ProcessingJob[] = [];
  isLoading = false;
  isRetryingJobId: number | null = null;
  errorMessage = '';

  constructor(
    private processingJobsService: ProcessingJobsService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['orderId'] && this.orderId) {
      this.loadProcessingJobs();
    }
  }

  loadProcessingJobs(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.processingJobsService.getJobsForOrder(this.orderId).subscribe({
      next: (jobs: ProcessingJob[]) => {
        this.processingJobs = jobs;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.processingJobs = [];
        this.errorMessage = 'Failed to load processing jobs.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  canRetryJob(job: ProcessingJob): boolean {
    return job.status === 'Failed' && this.authService.hasRole('Operations', 'Admin');
  }

  shouldShowMissingJobsWarning(): boolean {
    return !this.isLoading &&
      !this.errorMessage &&
      this.processingJobs.length === 0 &&
      (this.orderStatusId === 4 || this.orderStatusId === 5);
  }

  retryJob(job: ProcessingJob): void {
    if (!this.canRetryJob(job) || this.isRetryingJobId) {
      return;
    }

    this.isRetryingJobId = job.processingJobId;
    this.errorMessage = '';

    this.processingJobsService.retryJob(job.processingJobId).subscribe({
      next: () => {
        this.isRetryingJobId = null;
        this.loadProcessingJobs();
      },
      error: (err) => {
        this.errorMessage =
          err.error?.message || err.error || 'Failed to retry processing job.';
        this.isRetryingJobId = null;
        this.cdr.detectChanges();
      }
    });
  }

  getJobStatusClass(status: string): string {
    switch (status) {
      case 'Completed':
        return 'app-badge app-badge--success';
      case 'Processing':
        return 'app-badge app-badge--info';
      case 'Queued':
        return 'app-badge app-badge--warning';
      case 'Failed':
        return 'app-badge app-badge--danger';
      default:
        return 'app-badge app-badge--neutral';
    }
  }
}
