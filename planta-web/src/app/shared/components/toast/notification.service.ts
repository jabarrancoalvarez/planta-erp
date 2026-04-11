import { Injectable, signal } from '@angular/core';

export interface Toast {
  id: number;
  message: string;
  type: 'success' | 'error' | 'info';
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private nextId = 0;
  private readonly MAX_TOASTS = 5;

  readonly toasts = signal<Toast[]>([]);

  success(message: string): void {
    this.add(message, 'success');
  }

  error(message: string): void {
    this.add(message, 'error');
  }

  info(message: string): void {
    this.add(message, 'info');
  }

  dismiss(id: number): void {
    this.toasts.update(list => list.filter(t => t.id !== id));
  }

  private add(message: string, type: Toast['type']): void {
    const id = this.nextId++;
    this.toasts.update(list => {
      const next = [...list, { id, message, type }];
      return next.length > this.MAX_TOASTS ? next.slice(next.length - this.MAX_TOASTS) : next;
    });
    setTimeout(() => this.dismiss(id), 4000);
  }
}
