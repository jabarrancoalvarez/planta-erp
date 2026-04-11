import { Component, input, output, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-pagination',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="pagination">
      <button class="btn-outline" [disabled]="currentPage() <= 1" (click)="prev()">
        &#8592; Anterior
      </button>
      <span class="pagination__info">
        Pagina {{ currentPage() }} de {{ totalPages() }}
      </span>
      <button class="btn-outline" [disabled]="currentPage() >= totalPages()" (click)="next()">
        Siguiente &#8594;
      </button>
    </div>
  `,
  styles: [`
    .pagination {
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 1rem;
      padding: 0.75rem 0;
    }

    .pagination__info {
      font-size: 0.875rem;
      color: var(--color-gray-500, #6b7280);
      white-space: nowrap;
    }

    .btn-outline {
      display: inline-flex;
      align-items: center;
      gap: 0.375rem;
      padding: 0.5rem 0.875rem;
      background: white;
      color: var(--color-gray-700, #374151);
      border: 1.5px solid var(--color-gray-300, #d1d5db);
      border-radius: var(--radius-md, 8px);
      font-size: 0.8rem;
      font-weight: 600;
      cursor: pointer;
      transition: all 150ms ease;
    }

    .btn-outline:hover:not(:disabled) {
      background: var(--color-gray-100, #f3f4f6);
    }

    .btn-outline:disabled {
      opacity: 0.4;
      cursor: not-allowed;
    }
  `],
})
export class PaginationComponent {
  readonly currentPage = input.required<number>();
  readonly totalPages = input.required<number>();
  readonly totalCount = input<number>(0);
  readonly pageChange = output<number>();

  prev(): void {
    const p = this.currentPage();
    if (p > 1) this.pageChange.emit(p - 1);
  }

  next(): void {
    const p = this.currentPage();
    if (p < this.totalPages()) this.pageChange.emit(p + 1);
  }
}
