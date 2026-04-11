import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CalidadService, InspeccionListDto } from '../../../core/services/calidad.service';

@Component({
  selector: 'app-inspecciones-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="page">
      <div class="page__header">
        <div>
          <h1 class="page__title">Inspecciones</h1>
          <p class="page__subtitle">Registro de inspecciones de calidad</p>
        </div>
        <button class="btn-primary">+ Nueva Inspeccion</button>
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando inspecciones...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Fecha</th>
                <th>Origen</th>
                <th>Resultado</th>
              </tr>
            </thead>
            <tbody>
              @for (item of items(); track item.id) {
                <tr>
                  <td>{{ item.fechaInspeccion | date:'dd/MM/yyyy' }}</td>
                  <td>{{ item.origenInspeccion }}</td>
                  <td>
                    <span class="badge"
                      [class.badge--success]="item.resultadoInspeccion === 'Aprobada'"
                      [class.badge--danger]="item.resultadoInspeccion === 'Rechazada'"
                      [class.badge--warning]="!item.resultadoInspeccion">
                      {{ item.resultadoInspeccion ?? 'Pendiente' }}
                    </span>
                  </td>
                </tr>
              } @empty {
                <tr><td colspan="3" class="empty-state">No se encontraron resultados</td></tr>
              }
            </tbody>
          </table>
        </div>

        <div class="page__footer">{{ items().length }} de {{ totalCount() }} registros</div>
      }
    </div>
  `,
})
export class InspeccionesListComponent implements OnInit {
  private svc = inject(CalidadService);

  readonly items = signal<InspeccionListDto[]>([]);
  readonly totalCount = signal(0);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.loading.set(true);
    this.svc.listInspecciones().subscribe({
      next: (res) => {
        this.items.set(res.items);
        this.totalCount.set(res.totalCount);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err?.error?.message ?? 'Error al cargar inspecciones');
        this.loading.set(false);
      },
    });
  }
}
