import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InventarioService, MovimientoStockDto } from '../../../core/services/inventario.service';

@Component({
  selector: 'app-movimientos-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="page">
      <div class="page__header">
        <div>
          <h1 class="page__title">Movimientos de Stock</h1>
          <p class="page__subtitle">Registro de entradas, salidas, ajustes y transferencias</p>
        </div>
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando movimientos...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Fecha</th>
                <th>Tipo</th>
                <th>Cantidad</th>
                <th>Anterior</th>
                <th>Posterior</th>
                <th>Referencia</th>
              </tr>
            </thead>
            <tbody>
              @for (item of items(); track item.id) {
                <tr>
                  <td>{{ item.createdAt | date:'dd/MM/yyyy HH:mm' }}</td>
                  <td>
                    <span class="badge"
                      [class.badge--success]="item.tipo === 'Entrada'"
                      [class.badge--danger]="item.tipo === 'Salida'"
                      [class.badge--warning]="item.tipo === 'Ajuste'"
                      [class.badge--info]="item.tipo === 'Transferencia'">
                      {{ item.tipo }}
                    </span>
                  </td>
                  <td>{{ item.cantidad > 0 ? '+' : '' }}{{ item.cantidad }}</td>
                  <td>{{ item.cantidadAnterior }}</td>
                  <td>{{ item.cantidadPosterior }}</td>
                  <td><code>{{ item.referencia ?? '—' }}</code></td>
                </tr>
              } @empty {
                <tr><td colspan="6" class="empty-state">No se encontraron resultados</td></tr>
              }
            </tbody>
          </table>
        </div>

        <div class="page__footer">{{ items().length }} de {{ totalCount() }} registros</div>
      }
    </div>
  `,
})
export class MovimientosListComponent implements OnInit {
  private svc = inject(InventarioService);

  readonly items = signal<MovimientoStockDto[]>([]);
  readonly totalCount = signal(0);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.loading.set(true);
    this.svc.listMovimientos().subscribe({
      next: (res) => {
        this.items.set(res.items);
        this.totalCount.set(res.totalCount);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err?.error?.message ?? 'Error al cargar movimientos');
        this.loading.set(false);
      },
    });
  }
}
