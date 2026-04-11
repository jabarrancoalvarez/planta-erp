import { Component, signal, computed, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InventarioService, AlertaStockDto } from '../../../core/services/inventario.service';

@Component({
  selector: 'app-alertas-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="page">
      <div class="page__header">
        <div>
          <h1 class="page__title">Alertas de Stock</h1>
          <p class="page__subtitle">Configuracion de alertas de stock minimo</p>
        </div>
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando alertas...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Stock Minimo</th>
                <th>Stock Maximo</th>
                <th>Punto Reorden</th>
                <th>Cantidad Reorden</th>
                <th>Auto Reorden</th>
                <th>Activa</th>
              </tr>
            </thead>
            <tbody>
              @for (item of items(); track item.id) {
                <tr>
                  <td>{{ item.stockMinimo }}</td>
                  <td>{{ item.stockMaximo }}</td>
                  <td>{{ item.puntoReorden }}</td>
                  <td>{{ item.cantidadReorden }}</td>
                  <td>{{ item.autoReorden ? 'Si' : 'No' }}</td>
                  <td>
                    <span class="active-dot" [class.active-dot--active]="item.activa" [class.active-dot--inactive]="!item.activa"></span>
                    {{ item.activa ? 'Activa' : 'Inactiva' }}
                  </td>
                </tr>
              } @empty {
                <tr><td colspan="6" class="empty-state">No hay alertas configuradas</td></tr>
              }
            </tbody>
          </table>
        </div>

        <div class="page__footer">{{ items().length }} alertas</div>
      }
    </div>
  `,
})
export class AlertasListComponent implements OnInit {
  private svc = inject(InventarioService);

  readonly items = signal<AlertaStockDto[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.loading.set(true);
    this.svc.listAlertas().subscribe({
      next: (data) => {
        this.items.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err?.error?.message ?? 'Error al cargar alertas');
        this.loading.set(false);
      },
    });
  }
}
