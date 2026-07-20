export type ApiErrorResponse = {
  error?: {
    detail?: string;
    message?: string;
    error?: string;
  } | string;
};

export function getApiErrorMessage(error: ApiErrorResponse, fallback: string): string {
  if (typeof error.error === 'string') {
    return error.error;
  }

  return error.error?.detail
    || error.error?.message
    || error.error?.error
    || fallback;
}
