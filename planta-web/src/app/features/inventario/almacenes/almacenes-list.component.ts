import { Component, signal, computed, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { InventarioService, AlmacenListDto } from '../../../core/services/inventario.service';

@Component({
  selector: 'app-almacenes-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="page">
      <div class="page__header">
        <div>
          <h1 class="page__title">Almacenes</h1>
          <p class="page__subtitle">Gestion de almacenes y ubicaciones</p>
        </div>
        <button class="btn-primary">+ Nuevo Almacen</button>
      </div>

      <div class="page__filters">
        <input class="filter-input" placeholder="Buscar almacen..."
               (input)="onSearch($event)" />
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando almacenes...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Nombre</th>
                <th>Direccion</th>
                <th>Principal</th>
                <th>Ubicaciones</th>
              </tr>
            </thead>
            <tbody>
              @for (item of filtered(); track item.id) {
                <tr style="cursor:pointer;" (click)="goToDetail(item.id)">
                  <td>{{ item.nombre }}</td>
                  <td>{{ item.direccion ?? '—' }}</td>
                  <td>{{ item.esPrincipal ? 'Si' : 'No' }}</td>
                  <td>{{ item.totalUbicaciones }}</td>
                </tr>
              } @empty {
                <tr><td colspan="4" class="empty-state">No se encontraron resultados</td></tr>
              }
            </tbody>
          </table>
        </div>

        <div class="page__footer">{{ filtered().length }} de {{ allItems().length }} registros</div>
      }
    </div>
  `,
})
export class AlmacenesListComponent implements OnInit {
  private svc = inject(InventarioService);
  private router = inject(Router);

  readonly allItems = signal<AlmacenListDto[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly searchTerm = signal('');
  readonly filtered = computed(() => {
    const term = this.searchTerm().toLowerCase();
    if (!term) return this.allItems();
    return this.allItems().filter(a =>
      a.nombre.toLowerCase().includes(term)
    );
  });

  ngOnInit(): void {
    this.loading.set(true);
    this.svc.listAlmacenes().subscribe({
      next: (data) => {
        this.allItems.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err?.error?.message ?? 'Error al cargar almacenes');
        this.loading.set(false);
      },
    });
  }

  onSearch(event: Event): void {
    this.searchTerm.set((event.target as HTMLInputElement).value);
  }

  goToDetail(id: string): void {
    this.router.navigate(['/app/inventario/almacenes', id]);
  }
}
