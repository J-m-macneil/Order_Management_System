export interface ProcessingJob {
  processingJobId: number;
  orderId: number;
  jobType: string;
  status: string;
  attemptCount: number;
  maxAttempts: number;
  errorMessage?: string | null;
  createdAt: string;
  startedAt?: string | null;
  completedAt?: string | null;
  failedAt?: string | null;
  lastRetryAt?: string | null;
  nextAttemptAt?: string | null;
  payloadJson?: string | null;
}
