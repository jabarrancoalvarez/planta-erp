import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuditoriaService, AuditEntryDto } from '../../core/services/auditoria.service';

@Component({
  selector: 'app-auditoria-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule, DatePipe],
  template: `
    <div class="list-page">
      <div class="list-page__header">
        <h1 class="list-page__title">Auditoría</h1>
      </div>
      <p class="page__subtitle">Historial de cambios realizados en el sistema</p>

      <div class="filters-bar">
        <input [(ngModel)]="filterEntity" (ngModelChange)="load()" placeholder="Tipo entidad (ej: Producto)" />
        <input [(ngModel)]="filterUser" (ngModelChange)="load()" placeholder="Usuario" />
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Fecha</th>
                <th>Usuario</th>
                <th>Acción</th>
                <th>Entidad</th>
                <th>ID</th>
                <th>IP</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              @for (e of items(); track e.id) {
                <tr>
                  <td>{{ e.timestamp | date:'dd/MM/yyyy HH:mm:ss' }}</td>
                  <td>{{ e.userId }}</td>
                  <td><span class="badge">{{ e.action }}</span></td>
                  <td>{{ e.entityType }}</td>
                  <td><code>{{ e.entityId }}</code></td>
                  <td>{{ e.ipAddress ?? '---' }}</td>
                  <td>
                    <button class="btn-outline btn-sm" (click)="toggleDetail(e.id)">
                      {{ expandedId() === e.id ? 'Ocultar' : 'Ver cambios' }}
                    </button>
                  </td>
                </tr>
                @if (expandedId() === e.id) {
                  <tr>
                    <td colspan="7">
                      <div style="display:grid; grid-template-columns:1fr 1fr; gap:1rem;">
                        <div>
                          <strong>Antes:</strong>
                          <pre style="background:#f8f8f8;padding:0.5rem;overflow:auto;max-height:200px;">{{ e.oldValues ?? '—' }}</pre>
                        </div>
                        <div>
                          <strong>Después:</strong>
                          <pre style="background:#f8f8f8;padding:0.5rem;overflow:auto;max-height:200px;">{{ e.newValues ?? '—' }}</pre>
                        </div>
                      </div>
                    </td>
                  </tr>
                }
              } @empty {
                <tr><td colspan="7" class="empty-state">Sin registros</td></tr>
              }
            </tbody>
          </table>
        </div>
        <div class="page__footer">{{ items().length }} de {{ totalCount() }} registros</div>
      }
    </div>
  `,
})
export class AuditoriaListComponent implements OnInit {
  private svc = inject(AuditoriaService);

  readonly items = signal<AuditEntryDto[]>([]);
  readonly totalCount = signal(0);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly expandedId = signal<string | null>(null);

  filterEntity = '';
  filterUser = '';
  private searchTimeout: any;

  ngOnInit(): void { this.load(); }

  toggleDetail(id: string): void {
    this.expandedId.update(v => v === id ? null : id);
  }

  load(): void {
    clearTimeout(this.searchTimeout);
    this.searchTimeout = setTimeout(() => {
      this.loading.set(true);
      this.error.set(null);
      this.svc.list({
        entityType: this.filterEntity || undefined,
        userId: this.filterUser || undefined,
        page: 1,
        pageSize: 100,
      }).subscribe({
        next: (r) => { this.items.set(r.data); this.totalCount.set(r.totalCount); this.loading.set(false); },
        error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar auditoría'); this.loading.set(false); },
      });
    }, 300);
  }
}
