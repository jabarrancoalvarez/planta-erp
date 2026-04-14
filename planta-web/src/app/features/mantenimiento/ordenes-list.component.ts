import { Component, ChangeDetectionStrategy, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MantenimientoService, OrdenTrabajoListDto } from '../../core/services/mantenimiento.service';

@Component({
  selector: 'app-ordenes-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="page">
      <div class="page__header">
        <h1 class="page__title">Órdenes de Trabajo</h1>
        <p class="page__subtitle">Mantenimiento preventivo, correctivo y predictivo</p>
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando...</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Código</th>
                <th>Título</th>
                <th>Tipo</th>
                <th>Estado</th>
                <th>Prioridad</th>
                <th>Planificada</th>
              </tr>
            </thead>
            <tbody>
              @for (ot of items(); track ot.id) {
                <tr>
                  <td><code>{{ ot.codigo }}</code></td>
                  <td>{{ ot.titulo }}</td>
                  <td>{{ ot.tipo }}</td>
                  <td>{{ ot.estado }}</td>
                  <td>{{ ot.prioridad }}</td>
                  <td>{{ ot.fechaPlanificada | date:'dd/MM/yyyy' }}</td>
                </tr>
              } @empty {
                <tr><td colspan="6" class="empty-state">Sin órdenes de trabajo</td></tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `,
})
export class OrdenesListComponent implements OnInit {
  private svc = inject(MantenimientoService);
  readonly items = signal<OrdenTrabajoListDto[]>([]);
  readonly loading = signal(true);

  ngOnInit(): void {
    this.svc.listOrdenes({ pageSize: 50 }).subscribe({
      next: (res) => { this.items.set(res.items); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }
}
