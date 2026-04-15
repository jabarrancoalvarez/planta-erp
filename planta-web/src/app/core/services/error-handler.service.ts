import { ErrorHandler, Injectable, inject } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class GlobalErrorHandler implements ErrorHandler {
  private http = inject(HttpClient);
  private reporting = false;

  handleError(error: any): void {
    console.error('[GlobalErrorHandler]', error);

    if (error instanceof HttpErrorResponse) return;
    if (this.reporting) return;

    this.reporting = true;
    try {
      const payload = {
        message: error?.message ?? String(error),
        stack: error?.stack ?? null,
        url: typeof window !== 'undefined' ? window.location.href : null,
        userAgent: typeof navigator !== 'undefined' ? navigator.userAgent : null,
        timestamp: new Date().toISOString(),
      };
      this.http.post(`${environment.apiUrl}/sistema/frontend-error`, payload).subscribe({
        next: () => { this.reporting = false; },
        error: () => { this.reporting = false; },
      });
    } catch {
      this.reporting = false;
    }
  }
}
