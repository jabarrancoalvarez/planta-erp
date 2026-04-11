import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VentasService, ExpedicionListDto } from '../../../core/services/ventas.service';

@Component({
  selector: 'app-expediciones-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="page">
      <div class="page__header">
        <div>
          <h1 class="page__title">Expediciones</h1>
          <p class="page__subtitle">Control de envios y entregas</p>
        </div>
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando expediciones...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Pedido</th>
                <th>Fecha</th>
                <th>Num. Seguimiento</th>
                <th>Transportista</th>
                <th>Estado</th>
              </tr>
            </thead>
            <tbody>
              @for (item of items(); track item.id) {
                <tr>
                  <td><code>{{ item.codigoPedido }}</code></td>
                  <td>{{ item.fechaExpedicion | date:'dd/MM/yyyy' }}</td>
                  <td>{{ item.numeroSeguimiento ?? '—' }}</td>
                  <td>{{ item.transportista ?? '—' }}</td>
                  <td>
                    <span class="badge"
                      [class.badge--success]="item.estadoExpedicion === 'Entregada'"
                      [class.badge--info]="item.estadoExpedicion === 'EnTransito'"
                      [class.badge--warning]="item.estadoExpedicion === 'Pendiente' || item.estadoExpedicion === 'Preparada'">
                      {{ item.estadoExpedicion }}
                    </span>
                  </td>
                </tr>
              } @empty {
                <tr><td colspan="5" class="empty-state">No se encontraron resultados</td></tr>
              }
            </tbody>
          </table>
        </div>

        <div class="page__footer">{{ items().length }} de {{ totalCount() }} registros</div>
      }
    </div>
  `,
})
export class ExpedicionesListComponent implements OnInit {
  private svc = inject(VentasService);

  readonly items = signal<ExpedicionListDto[]>([]);
  readonly totalCount = signal(0);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.loading.set(true);
    this.svc.listExpediciones().subscribe({
      next: (res) => {
        this.items.set(res.items);
        this.totalCount.set(res.totalCount);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err?.error?.message ?? 'Error al cargar expediciones');
        this.loading.set(false);
      },
    });
  }
}
