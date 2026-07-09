# Background Processing Workflow

When an order is approved, the application creates the required processing jobs up front:

1. Generate order summary document.
2. Generate SDS bundle, only when an order contains products that require SDS documents.
3. Push the order to the logistics provider.

The hosted background service only wakes up on a timer and asks the processing job processor to handle the next batch.

The processor handles the job lifecycle:

- pick the next queued job for each order by workflow priority
- move the order into `In Processing`
- mark the job as `Processing`
- delegate the work to the matching job handler
- mark the job as `Completed` or `Failed`
- cancel later jobs when a required job fails
- requeue later cancelled jobs after a failed job is retried successfully

If all jobs complete, the logistics job moves the order to `Awaiting Dispatch`.

If a required job fails, the order moves to `Failed`, later jobs are cancelled, and Operations/Admin users can retry the failed job while it is still within the retry limit.
