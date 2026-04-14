import { Component, ChangeDetectionStrategy, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivosService, ActivoListDto } from '../../core/services/activos.service';

@Component({
  selector: 'app-activos-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="page">
      <div class="page__header">
        <h1 class="page__title">Activos</h1>
        <p class="page__subtitle">Inventario de máquinas, líneas, herramientas e instalaciones</p>
      </div>

      <div class="page__filters">
        <input class="filter-input" placeholder="Buscar por código o nombre..." (input)="onSearch($event)" />
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando activos...</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Código</th>
                <th>Nombre</th>
                <th>Tipo</th>
                <th>Criticidad</th>
                <th>Estado</th>
                <th>Ubicación</th>
              </tr>
            </thead>
            <tbody>
              @for (a of items(); track a.id) {
                <tr>
                  <td><code>{{ a.codigo }}</code></td>
                  <td>{{ a.nombre }}</td>
                  <td>{{ a.tipo }}</td>
                  <td>{{ a.criticidad }}</td>
                  <td>{{ a.estado }}</td>
                  <td>{{ a.ubicacion }}</td>
                </tr>
              } @empty {
                <tr><td colspan="6" class="empty-state">No hay activos</td></tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `,
})
export class ActivosListComponent implements OnInit {
  private svc = inject(ActivosService);
  readonly items = signal<ActivoListDto[]>([]);
  readonly loading = signal(true);
  private searchTimeout: any;

  ngOnInit(): void { this.load(); }

  onSearch(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    clearTimeout(this.searchTimeout);
    this.searchTimeout = setTimeout(() => this.load(value), 300);
  }

  private load(search?: string): void {
    this.loading.set(true);
    this.svc.listActivos({ search, pageSize: 50 }).subscribe({
      next: (res) => { this.items.set(res.items); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }
}
