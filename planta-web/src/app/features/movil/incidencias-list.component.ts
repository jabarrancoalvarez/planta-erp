import { Component, ChangeDetectionStrategy, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { IncidenciasService, IncidenciaListDto } from '../../core/services/incidencias.service';

@Component({
  selector: 'app-incidencias-list-movil',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="movil-page">
      <header class="movil-page__header">
        <a class="back" routerLink="..">←</a>
        <h1>Incidencias Abiertas</h1>
      </header>

      @if (loading()) {
        <div class="loading">Cargando...</div>
      } @else {
        <div class="list">
          @for (inc of items(); track inc.id) {
            <div class="inc-card" [class.inc-card--critica]="inc.severidad === 'Critica'">
              <div class="inc-card__top">
                <code>{{ inc.codigo }}</code>
                <span class="badge badge--{{ inc.severidad.toLowerCase() }}">{{ inc.severidad }}</span>
              </div>
              <h3>{{ inc.titulo }}</h3>
              <div class="inc-card__meta">
                <span>{{ inc.tipo }}</span> · <span>{{ inc.estado }}</span> · <span>{{ inc.fechaApertura | date:'dd/MM HH:mm' }}</span>
                @if (inc.paradaLinea) { <span class="parada">LÍNEA PARADA</span> }
              </div>
            </div>
          } @empty {
            <div class="empty">No hay incidencias abiertas</div>
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
    .inc-card { background: white; padding: 1rem; border-radius: 12px; box-shadow: 0 2px 6px rgba(0,0,0,0.05); border-left: 4px solid #fbbf24; }
    .inc-card--critica { border-left-color: #dc2626; }
    .inc-card__top { display: flex; justify-content: space-between; align-items: center; margin-bottom: 0.5rem; }
    .inc-card h3 { margin: 0.25rem 0; font-size: 1rem; }
    .inc-card__meta { color: #666; font-size: 0.85rem; }
    .parada { background: #dc2626; color: white; padding: 0.15rem 0.5rem; border-radius: 4px; font-weight: 600; font-size: 0.7rem; margin-left: 0.25rem; }
    .badge { padding: 0.2rem 0.6rem; border-radius: 999px; font-size: 0.75rem; font-weight: 600; background: #eee; }
    .badge--critica { background: #fee; color: #dc2626; }
    .badge--alta { background: #fff3e0; color: #e65100; }
    .loading, .empty { text-align: center; padding: 2rem; color: #666; }
  `],
})
export class IncidenciasListComponent implements OnInit {
  private svc = inject(IncidenciasService);

  readonly items = signal<IncidenciaListDto[]>([]);
  readonly loading = signal(true);

  ngOnInit(): void {
    this.svc.listIncidencias({ pageSize: 50 }).subscribe({
      next: (res) => {
        const abiertas = res.items.filter(i => i.estado !== 'Cerrada' && i.estado !== 'Descartada');
        this.items.set(abiertas);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
