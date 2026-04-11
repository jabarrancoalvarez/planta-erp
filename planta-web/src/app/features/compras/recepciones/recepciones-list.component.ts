import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ComprasService, RecepcionListDto } from '../../../core/services/compras.service';

@Component({
  selector: 'app-recepciones-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="page">
      <div class="page__header">
        <div>
          <h1 class="page__title">Recepciones</h1>
          <p class="page__subtitle">Control de recepciones de material</p>
        </div>
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando recepciones...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Orden Compra</th>
                <th>Fecha Recepcion</th>
                <th>Num. Albaran</th>
                <th>Estado</th>
              </tr>
            </thead>
            <tbody>
              @for (item of items(); track item.id) {
                <tr>
                  <td><code>{{ item.codigoOC }}</code></td>
                  <td>{{ item.fechaRecepcion | date:'dd/MM/yyyy' }}</td>
                  <td>{{ item.numeroAlbaran ?? '—' }}</td>
                  <td>
                    <span class="badge"
                      [class.badge--success]="item.estadoRecepcion === 'Completada' || item.estadoRecepcion === 'Aceptada'"
                      [class.badge--warning]="item.estadoRecepcion === 'Pendiente'"
                      [class.badge--danger]="item.estadoRecepcion === 'Rechazada'">
                      {{ item.estadoRecepcion }}
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
export class RecepcionesListComponent implements OnInit {
  private svc = inject(ComprasService);

  readonly items = signal<RecepcionListDto[]>([]);
  readonly totalCount = signal(0);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.loading.set(true);
    this.svc.listRecepciones().subscribe({
      next: (res) => {
        this.items.set(res.items);
        this.totalCount.set(res.totalCount);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err?.error?.message ?? 'Error al cargar recepciones');
        this.loading.set(false);
      },
    });
  }
}
