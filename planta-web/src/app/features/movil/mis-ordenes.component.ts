import { Component, ChangeDetectionStrategy, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MantenimientoService, OrdenTrabajoListDto } from '../../core/services/mantenimiento.service';

@Component({
  selector: 'app-mis-ordenes',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="movil-page">
      <header class="movil-page__header">
        <a class="back" routerLink="..">←</a>
        <h1>Mis Órdenes de Trabajo</h1>
      </header>

      @if (loading()) {
        <div class="loading">Cargando...</div>
      } @else {
        <div class="list">
          @for (ot of items(); track ot.id) {
            <div class="ot-card" [class.ot-card--urgente]="ot.prioridad === 'Urgente' || ot.prioridad === 'Alta'">
              <div class="ot-card__top">
                <code>{{ ot.codigo }}</code>
                <span class="badge badge--{{ ot.prioridad.toLowerCase() }}">{{ ot.prioridad }}</span>
              </div>
              <h3>{{ ot.titulo }}</h3>
              <div class="ot-card__meta">
                <span>{{ ot.tipo }}</span> · <span>{{ ot.estado }}</span>
                @if (ot.fechaPlanificada) {
                  · <span>{{ ot.fechaPlanificada | date:'dd/MM HH:mm' }}</span>
                }
              </div>
            </div>
          } @empty {
            <div class="empty">No hay órdenes pendientes</div>
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
    .list { display: flex; flex-direction: column; gap: 0.75rem; }
    .ot-card { background: white; padding: 1rem; border-radius: 12px; box-shadow: 0 2px 6px rgba(0,0,0,0.05); border-left: 4px solid #ddd; }
    .ot-card--urgente { border-left-color: #dc2626; }
    .ot-card__top { display: flex; justify-content: space-between; align-items: center; margin-bottom: 0.5rem; }
    .ot-card h3 { margin: 0.25rem 0; font-size: 1rem; }
    .ot-card__meta { color: #666; font-size: 0.85rem; }
    .badge { padding: 0.2rem 0.6rem; border-radius: 999px; font-size: 0.75rem; font-weight: 600; background: #eee; }
    .badge--urgente { background: #fee; color: #dc2626; }
    .badge--alta { background: #fff3e0; color: #e65100; }
    .loading, .empty { text-align: center; padding: 2rem; color: #666; }
  `],
})
export class MisOrdenesComponent implements OnInit {
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
