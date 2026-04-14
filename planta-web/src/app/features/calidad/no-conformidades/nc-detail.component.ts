import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { CalidadService, NCDetailDto } from '../../../core/services/calidad.service';
import { NotificationService } from '../../../shared/components/toast/notification.service';

@Component({
  selector: 'app-nc-detail',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="detail-page">
      <div class="detail-page__header">
        <button class="detail-page__back" (click)="goBack()">&larr; Volver</button>
        <h1 class="detail-page__title">No Conformidad</h1>
        @if (item()) {
          <button class="detail-page__back" style="margin-left:auto;background:#fee;color:#c00;" (click)="onDelete()">Eliminar</button>
        }
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando NC...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else if (item(); as nc) {
        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Informacion General</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Codigo</span>
              <span class="detail-page__field-value"><code>{{ nc.codigo }}</code></span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Origen</span>
              <span class="detail-page__field-value">{{ nc.origenInspeccion }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Severidad</span>
              <span class="detail-page__field-value">
                <span class="badge"
                  [class.badge--danger]="nc.severidadNC === 'Critica'"
                  [class.badge--warning]="nc.severidadNC === 'Mayor'"
                  [class.badge--neutral]="nc.severidadNC === 'Menor'">
                  {{ nc.severidadNC }}
                </span>
              </span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Estado</span>
              <span class="detail-page__field-value">
                <span class="badge"
                  [class.badge--danger]="nc.estadoNoConformidad === 'Abierta'"
                  [class.badge--info]="nc.estadoNoConformidad === 'EnInvestigacion'"
                  [class.badge--warning]="nc.estadoNoConformidad === 'AccionDefinida'"
                  [class.badge--success]="nc.estadoNoConformidad === 'Resuelta' || nc.estadoNoConformidad === 'Cerrada'">
                  {{ nc.estadoNoConformidad }}
                </span>
              </span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Fecha Deteccion</span>
              <span class="detail-page__field-value">{{ nc.fechaDeteccion | date:'dd/MM/yyyy' }}</span>
            </div>
          </div>
        </div>

        @if (nc.descripcion) {
          <div class="detail-page__section">
            <h2 class="detail-page__section-title">Descripcion</h2>
            <p>{{ nc.descripcion }}</p>
          </div>
        }

        @if (nc.causaRaiz) {
          <div class="detail-page__section">
            <h2 class="detail-page__section-title">Causa Raiz</h2>
            <p>{{ nc.causaRaiz }}</p>
          </div>
        }

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Acciones Correctivas ({{ nc.accionesCorrectivas.length }})</h2>
          @if (nc.accionesCorrectivas.length > 0) {
            <div class="table-container">
              <table class="data-table">
                <thead>
                  <tr>
                    <th>Descripcion</th>
                    <th>Responsable</th>
                    <th>Fecha Limite</th>
                    <th>Estado</th>
                  </tr>
                </thead>
                <tbody>
                  @for (a of nc.accionesCorrectivas; track a.id) {
                    <tr>
                      <td>{{ a.descripcion }}</td>
                      <td>{{ a.responsable ?? '---' }}</td>
                      <td>{{ a.fechaLimite ? (a.fechaLimite | date:'dd/MM/yyyy') : '---' }}</td>
                      <td>
                        <span class="badge"
                          [class.badge--warning]="a.estado === 'Pendiente'"
                          [class.badge--info]="a.estado === 'EnCurso'"
                          [class.badge--success]="a.estado === 'Completada'">
                          {{ a.estado }}
                        </span>
                      </td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          } @else {
            <p class="empty-state">Sin acciones correctivas definidas</p>
          }
        </div>

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Cambiar Estado</h2>
          <div class="detail-page__actions">
            @for (transition of getAvailableTransitions(nc.estadoNoConformidad); track transition) {
              <button class="btn-outline btn-sm" (click)="changeState(transition)">
                {{ transition }}
              </button>
            }
          </div>
        </div>
      }
    </div>
  `,
})
export class NCDetailComponent implements OnInit {
  private svc = inject(CalidadService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private notify = inject(NotificationService);

  readonly item = signal<NCDetailDto | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  private id = '';

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id')!;
    this.loadDetail();
  }

  goBack(): void { this.router.navigate(['/app/calidad/no-conformidades']); }

  onDelete(): void {
    const nc = this.item();
    if (!nc) return;
    if (!confirm(`¿Eliminar no conformidad "${nc.codigo}"?`)) return;
    this.svc.deleteNC(this.id).subscribe({
      next: () => this.router.navigate(['/app/calidad/no-conformidades']),
      error: (err) => this.error.set(err?.error?.message ?? 'Error al eliminar'),
    });
  }

  getAvailableTransitions(state: string): string[] {
    const map: Record<string, string[]> = {
      'Abierta': ['EnInvestigacion'],
      'EnInvestigacion': ['AccionDefinida'],
      'AccionDefinida': ['Resuelta'],
      'Resuelta': ['Cerrada'],
    };
    return map[state] ?? [];
  }

  changeState(estadoDestino: string): void {
    this.svc.cambiarEstadoNC(this.id, estadoDestino).subscribe({
      next: () => { this.notify.success(`Estado cambiado a ${estadoDestino}`); this.loadDetail(); },
      error: (err) => this.notify.error(err?.error?.message ?? 'Error al cambiar estado'),
    });
  }

  private loadDetail(): void {
    this.loading.set(true);
    this.svc.getNC(this.id).subscribe({
      next: (data) => { this.item.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar NC'); this.loading.set(false); },
    });
  }
}
