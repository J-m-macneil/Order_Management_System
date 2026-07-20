import { apiBaseUrl } from '../config/api-url';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProcessingJob } from '../models/processing-job.model';

@Injectable({
  providedIn: 'root'
})
export class ProcessingJobsService {
  private readonly apiUrl = `${apiBaseUrl}/processing-jobs`;

  constructor(private http: HttpClient) { }

  getJobsForOrder(orderId: number): Observable<ProcessingJob[]> {
    return this.http.get<ProcessingJob[]>(`${this.apiUrl}/order/${orderId}`);
  }

  getFailedJobs(): Observable<ProcessingJob[]> {
    return this.http.get<ProcessingJob[]>(`${this.apiUrl}/failed`);
  }

  retryJob(jobId: number): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${jobId}/retry`, {});
  }
}
