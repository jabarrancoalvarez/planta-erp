import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ProduccionService, RutaListDto } from '../../../core/services/produccion.service';

@Component({
  selector: 'app-rutas-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="page">
      <div class="page__header">
        <div>
          <h1 class="page__title">Rutas de Produccion</h1>
          <p class="page__subtitle">Secuencias de operaciones para cada producto</p>
        </div>
        <button class="btn-primary">+ Nueva Ruta</button>
      </div>

      <div class="page__filters">
        <input class="filter-input" placeholder="Buscar por nombre..."
               (input)="onSearch($event)" />
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando rutas...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Nombre</th>
                <th>Num. Operaciones</th>
                <th>Estado</th>
              </tr>
            </thead>
            <tbody>
              @for (item of items(); track item.id) {
                <tr style="cursor:pointer;" (click)="goToDetail(item.id)">
                  <td>{{ item.nombre }}</td>
                  <td>{{ item.numeroOperaciones }}</td>
                  <td>
                    <span class="active-dot" [class.active-dot--active]="item.activa" [class.active-dot--inactive]="!item.activa"></span>
                    {{ item.activa ? 'Activa' : 'Inactiva' }}
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
export class RutasListComponent implements OnInit {
  private svc = inject(ProduccionService);
  private router = inject(Router);

  readonly items = signal<RutaListDto[]>([]);
  readonly totalCount = signal(0);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  private searchTimeout: any;

  ngOnInit(): void {
    this.load();
  }

  onSearch(event: Event): void {
    clearTimeout(this.searchTimeout);
    const term = (event.target as HTMLInputElement).value;
    this.searchTimeout = setTimeout(() => this.load(term), 300);
  }

  goToDetail(id: string): void {
    this.router.navigate(['/app/produccion/rutas', id]);
  }

  private load(search?: string): void {
    this.loading.set(true);
    this.error.set(null);
    this.svc.listRutas(search).subscribe({
      next: (res) => {
        this.items.set(res.items);
        this.totalCount.set(res.totalCount);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err?.error?.message ?? 'Error al cargar rutas');
        this.loading.set(false);
      },
    });
  }
}
