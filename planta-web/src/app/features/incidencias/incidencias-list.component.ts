import { Component, ChangeDetectionStrategy, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IncidenciasService, IncidenciaListDto } from '../../core/services/incidencias.service';

@Component({
  selector: 'app-incidencias-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="page">
      <div class="page__header">
        <h1 class="page__title">Incidencias</h1>
        <p class="page__subtitle">Avisos de avería, parada, seguridad y calidad</p>
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
                <th>Severidad</th>
                <th>Estado</th>
                <th>Apertura</th>
                <th>Parada</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              @for (inc of items(); track inc.id) {
                <tr>
                  <td><code>{{ inc.codigo }}</code></td>
                  <td>{{ inc.titulo }}</td>
                  <td>{{ inc.tipo }}</td>
                  <td>{{ inc.severidad }}</td>
                  <td>{{ inc.estado }}</td>
                  <td>{{ inc.fechaApertura | date:'dd/MM/yyyy HH:mm' }}</td>
                  <td>{{ inc.paradaLinea ? 'Sí' : '-' }}</td>
                  <td>
                    <button class="btn-outline btn-sm" style="background:#fee;color:#c00;" (click)="onDelete(inc)">Eliminar</button>
                  </td>
                </tr>
              } @empty {
                <tr><td colspan="8" class="empty-state">Sin incidencias</td></tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `,
})
export class IncidenciasListComponent implements OnInit {
  private svc = inject(IncidenciasService);
  readonly items = signal<IncidenciaListDto[]>([]);
  readonly loading = signal(true);

  ngOnInit(): void {
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.svc.listIncidencias({ pageSize: 50 }).subscribe({
      next: (res) => { this.items.set(res.items); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  onDelete(inc: IncidenciaListDto): void {
    if (!confirm(`¿Eliminar la incidencia ${inc.codigo}?`)) return;
    this.svc.deleteIncidencia(inc.id).subscribe({
      next: () => this.load(),
      error: () => alert('Error al eliminar la incidencia'),
    });
  }
}
