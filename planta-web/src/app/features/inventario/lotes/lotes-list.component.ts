import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InventarioService, LoteListDto } from '../../../core/services/inventario.service';

@Component({
  selector: 'app-lotes-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="page">
      <div class="page__header">
        <div>
          <h1 class="page__title">Lotes</h1>
          <p class="page__subtitle">Trazabilidad y gestion de lotes de materiales</p>
        </div>
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando lotes...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Codigo</th>
                <th>Cantidad Actual</th>
                <th>Fecha Caducidad</th>
                <th>Estado</th>
              </tr>
            </thead>
            <tbody>
              @for (item of items(); track item.id) {
                <tr>
                  <td><code>{{ item.codigo }}</code></td>
                  <td>{{ item.cantidadActual }}</td>
                  <td>{{ item.fechaCaducidad ? (item.fechaCaducidad | date:'dd/MM/yyyy') : '—' }}</td>
                  <td>
                    <span class="badge"
                      [class.badge--success]="item.estado === 'Disponible'"
                      [class.badge--info]="item.estado === 'EnInspeccion'"
                      [class.badge--warning]="item.estado === 'Cuarentena'">
                      {{ item.estado }}
                    </span>
                  </td>
                </tr>
              } @empty {
                <tr><td colspan="4" class="empty-state">No se encontraron resultados</td></tr>
              }
            </tbody>
          </table>
        </div>

        <div class="page__footer">{{ items().length }} de {{ totalCount() }} registros</div>
      }
    </div>
  `,
})
export class LotesListComponent implements OnInit {
  private svc = inject(InventarioService);

  readonly items = signal<LoteListDto[]>([]);
  readonly totalCount = signal(0);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.loading.set(true);
    this.svc.listLotes().subscribe({
      next: (res) => {
        this.items.set(res.items);
        this.totalCount.set(res.totalCount);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err?.error?.message ?? 'Error al cargar lotes');
        this.loading.set(false);
      },
    });
  }
}
