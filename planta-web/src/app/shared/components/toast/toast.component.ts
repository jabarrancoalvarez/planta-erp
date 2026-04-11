import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { NotificationService } from './notification.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="toast-container">
      @for (toast of notifications.toasts(); track toast.id) {
        <div class="toast" [class]="'toast--' + toast.type" (click)="notifications.dismiss(toast.id)">
          <span class="toast__icon">
            @switch (toast.type) {
              @case ('success') { &#10003; }
              @case ('error') { &#10007; }
              @case ('info') { &#9432; }
            }
          </span>
          <span class="toast__message">{{ toast.message }}</span>
        </div>
      }
    </div>
  `,
  styles: [`
    .toast-container {
      position: fixed;
      bottom: 1.5rem;
      right: 1.5rem;
      z-index: 9999;
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
      max-width: 380px;
    }

    .toast {
      display: flex;
      align-items: center;
      gap: 0.625rem;
      padding: 0.75rem 1rem;
      border-radius: var(--radius-md, 8px);
      font-size: 0.875rem;
      font-family: var(--font-body, 'Inter', sans-serif);
      color: white;
      cursor: pointer;
      box-shadow: var(--shadow-md, 0 4px 24px rgba(0,0,0,0.08));
      animation: toast-in 250ms ease forwards;
    }

    .toast--success { background: var(--color-success, #10b981); }
    .toast--error { background: var(--color-danger, #ef4444); }
    .toast--info { background: var(--color-primary, #2563eb); }

    .toast__icon {
      font-size: 1rem;
      flex-shrink: 0;
    }

    .toast__message {
      flex: 1;
      line-height: 1.4;
    }

    @keyframes toast-in {
      from {
        opacity: 0;
        transform: translateX(1rem);
      }
      to {
        opacity: 1;
        transform: translateX(0);
      }
    }
  `],
})
export class ToastComponent {
  readonly notifications = inject(NotificationService);
}
