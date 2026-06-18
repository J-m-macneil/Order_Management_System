import { Component, Input, OnChanges, SimpleChanges, ChangeDetectorRef } from '@angular/core';
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
    return job.status === 'Failed';
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
        return 'bg-green-100 dark:bg-green-900/20 text-green-700 dark:text-green-400';
      case 'Processing':
        return 'bg-blue-100 dark:bg-blue-900/20 text-blue-700 dark:text-blue-400';
      case 'Queued':
        return 'bg-amber-100 dark:bg-amber-900/20 text-amber-700 dark:text-amber-400';
      case 'Failed':
        return 'bg-red-100 dark:bg-red-900/20 text-red-700 dark:text-red-400';
      default:
        return 'bg-slate-100 dark:bg-slate-700 text-slate-700 dark:text-slate-300';
    }
  }
}
