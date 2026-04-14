import { Component, ChangeDetectionStrategy, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ActivosService, ActivoListDto } from '../../core/services/activos.service';

@Component({
  selector: 'app-activos-list-movil',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="movil-page">
      <header class="movil-page__header">
        <a class="back" routerLink="..">←</a>
        <h1>Activos</h1>
      </header>

      <input class="search" placeholder="Buscar..." (input)="onSearch($event)" />

      @if (loading()) {
        <div class="loading">Cargando...</div>
      } @else {
        <div class="list">
          @for (a of items(); track a.id) {
            <div class="activo-card">
              <div class="activo-card__top">
                <code>{{ a.codigo }}</code>
                <span class="estado estado--{{ a.estado.toLowerCase() }}">{{ a.estado }}</span>
              </div>
              <h3>{{ a.nombre }}</h3>
              <div class="meta">{{ a.tipo }} · {{ a.criticidad }} @if (a.ubicacion) { · {{ a.ubicacion }} }</div>
            </div>
          } @empty {
            <div class="empty">Sin resultados</div>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .movil-page { padding: 1rem; min-height: 100vh; background: #f5f5f7; }
    .movil-page__header { display: flex; align-items: center; gap: 0.75rem; margin-bottom: 1rem; }
    .movil-page__header h1 { margin: 0; font-size: 1.25rem; }
    .back { text-decoration: none; font-size: 1.5rem; color: #111; padding: 0.25rem 0.75rem; background: white; border-radius: 8px; }
    .search { width: 100%; padding: 0.75rem; border: 1px solid #ddd; border-radius: 8px; margin-bottom: 1rem; font-size: 1rem; }
    .list { display: flex; flex-direction: column; gap: 0.75rem; }
    .activo-card { background: white; padding: 1rem; border-radius: 12px; box-shadow: 0 2px 6px rgba(0,0,0,0.05); }
    .activo-card__top { display: flex; justify-content: space-between; align-items: center; margin-bottom: 0.25rem; }
    .activo-card h3 { margin: 0.25rem 0; font-size: 1rem; }
    .meta { color: #666; font-size: 0.85rem; }
    .estado { padding: 0.2rem 0.6rem; border-radius: 999px; font-size: 0.75rem; font-weight: 600; background: #eee; }
    .estado--operativo { background: #dcfce7; color: #166534; }
    .estado--enmantenimiento { background: #fef3c7; color: #92400e; }
    .estado--averiado { background: #fee; color: #dc2626; }
    .loading, .empty { text-align: center; padding: 2rem; color: #666; }
  `],
})
export class ActivosListMovilComponent implements OnInit {
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
