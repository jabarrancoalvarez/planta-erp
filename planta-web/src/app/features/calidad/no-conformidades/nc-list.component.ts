import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CalidadService, NCListDto } from '../../../core/services/calidad.service';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-nc-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, PaginationComponent],
  template: `
    <div class="page">
      <div class="page__header">
        <div>
          <h1 class="page__title">No Conformidades</h1>
          <p class="page__subtitle">Registro y seguimiento de desviaciones de calidad</p>
        </div>
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando no conformidades...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Codigo</th>
                <th>Origen</th>
                <th>Severidad</th>
                <th>Estado</th>
                <th>Fecha Deteccion</th>
              </tr>
            </thead>
            <tbody>
              @for (item of items(); track item.id) {
                <tr class="clickable-row" (click)="goToDetail(item.id)">
                  <td><code>{{ item.codigo }}</code></td>
                  <td>{{ item.origenInspeccion }}</td>
                  <td>
                    <span class="badge"
                      [class.badge--danger]="item.severidadNC === 'Critica'"
                      [class.badge--warning]="item.severidadNC === 'Mayor'"
                      [class.badge--neutral]="item.severidadNC === 'Menor'">
                      {{ item.severidadNC }}
                    </span>
                  </td>
                  <td>
                    <span class="badge"
                      [class.badge--danger]="item.estadoNoConformidad === 'Abierta'"
                      [class.badge--info]="item.estadoNoConformidad === 'EnInvestigacion'"
                      [class.badge--warning]="item.estadoNoConformidad === 'AccionDefinida'"
                      [class.badge--success]="item.estadoNoConformidad === 'Resuelta' || item.estadoNoConformidad === 'Cerrada'">
                      {{ item.estadoNoConformidad }}
                    </span>
                  </td>
                  <td>{{ item.fechaDeteccion | date:'dd/MM/yyyy' }}</td>
                </tr>
              } @empty {
                <tr><td colspan="5" class="empty-state">No se encontraron resultados</td></tr>
              }
            </tbody>
          </table>
        </div>

        @if (totalPages() > 1) {
          <app-pagination [currentPage]="currentPage()" [totalPages]="totalPages()" (pageChange)="onPageChange($event)" />
        }

        <div class="page__footer">{{ items().length }} de {{ totalCount() }} registros</div>
      }
    </div>
  `,
})
export class NcListComponent implements OnInit {
  private svc = inject(CalidadService);
  private router = inject(Router);

  readonly items = signal<NCListDto[]>([]);
  readonly totalCount = signal(0);
  readonly currentPage = signal(1);
  readonly totalPages = signal(1);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  ngOnInit(): void { this.load(); }

  onPageChange(page: number): void { this.currentPage.set(page); this.load(); }
  goToDetail(id: string): void { this.router.navigate(['/app/calidad/no-conformidades', id]); }

  private load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.svc.listNCs(undefined, undefined, this.currentPage()).subscribe({
      next: (res) => { this.items.set(res.items); this.totalCount.set(res.totalCount); this.totalPages.set(res.totalPages); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar no conformidades'); this.loading.set(false); },
    });
  }
}
