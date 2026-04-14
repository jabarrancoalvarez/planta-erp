import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ProduccionService, OFDetailDto } from '../../../core/services/produccion.service';
import { NotificationService } from '../../../shared/components/toast/notification.service';

@Component({
  selector: 'app-of-detail',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="detail-page">
      <div class="detail-page__header">
        <button class="detail-page__back" (click)="goBack()">&larr; Volver</button>
        <h1 class="detail-page__title">Orden de Fabricacion</h1>
        @if (item()) {
          <button class="detail-page__back" style="margin-left:auto;background:#fee;color:#c00;" (click)="onDelete()">Eliminar</button>
        }
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando orden...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else if (item(); as of) {
        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Informacion General</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Codigo</span>
              <span class="detail-page__field-value"><code>{{ of.codigoOF }}</code></span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Estado</span>
              <span class="detail-page__field-value">
                <span class="badge"
                  [class.badge--info]="of.estadoOF === 'Planificada'"
                  [class.badge--warning]="of.estadoOF === 'EnCurso' || of.estadoOF === 'EnPreparacion'"
                  [class.badge--success]="of.estadoOF === 'Completada'"
                  [class.badge--neutral]="of.estadoOF === 'Cancelada' || of.estadoOF === 'Pausada'">
                  {{ of.estadoOF }}
                </span>
              </span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Prioridad</span>
              <span class="detail-page__field-value">{{ of.prioridad }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Cantidad Planificada</span>
              <span class="detail-page__field-value">{{ of.cantidadPlanificada }} {{ of.unidadMedida }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Cantidad Producida</span>
              <span class="detail-page__field-value">{{ of.cantidadProducida }} {{ of.unidadMedida }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Fecha Inicio</span>
              <span class="detail-page__field-value">{{ of.fechaInicio ? (of.fechaInicio | date:'dd/MM/yyyy') : '---' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Fecha Fin Estimada</span>
              <span class="detail-page__field-value">{{ of.fechaFinEstimada ? (of.fechaFinEstimada | date:'dd/MM/yyyy') : '---' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Fecha Fin Real</span>
              <span class="detail-page__field-value">{{ of.fechaFinReal ? (of.fechaFinReal | date:'dd/MM/yyyy') : '---' }}</span>
            </div>
          </div>
        </div>

        @if (of.observaciones) {
          <div class="detail-page__section">
            <h2 class="detail-page__section-title">Observaciones</h2>
            <p>{{ of.observaciones }}</p>
          </div>
        }

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Cambiar Estado</h2>
          <div class="detail-page__actions">
            @for (transition of getAvailableTransitions(of.estadoOF); track transition) {
              <button
                class="btn-outline btn-sm"
                [class.btn-danger]="transition === 'Cancelada'"
                (click)="changeState(transition)">
                {{ transition }}
              </button>
            }
          </div>
        </div>
      }
    </div>
  `,
})
export class OFDetailComponent implements OnInit {
  private svc = inject(ProduccionService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private notify = inject(NotificationService);

  readonly item = signal<OFDetailDto | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  private id = '';

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id')!;
    this.loadDetail();
  }

  goBack(): void { this.router.navigate(['/app/produccion/ordenes']); }

  getAvailableTransitions(state: string): string[] {
    const map: Record<string, string[]> = {
      'Planificada': ['EnPreparacion', 'Cancelada'],
      'EnPreparacion': ['EnCurso', 'Cancelada'],
      'EnCurso': ['Pausada', 'Completada', 'Cancelada'],
      'Pausada': ['EnCurso', 'Cancelada'],
    };
    return map[state] ?? [];
  }

  changeState(estadoDestino: string): void {
    this.svc.cambiarEstadoOF(this.id, estadoDestino).subscribe({
      next: () => { this.notify.success(`Estado cambiado a ${estadoDestino}`); this.loadDetail(); },
      error: (err) => this.notify.error(err?.error?.message ?? 'Error al cambiar estado'),
    });
  }

  onDelete(): void {
    const of = this.item();
    if (!of) return;
    if (!confirm(`¿Eliminar orden de fabricación "${of.codigoOF}"?`)) return;
    this.svc.deleteOF(this.id).subscribe({
      next: () => this.router.navigate(['/app/produccion/ordenes']),
      error: (err) => this.notify.error(err?.error?.message ?? 'Error al eliminar'),
    });
  }

  private loadDetail(): void {
    this.loading.set(true);
    this.svc.getOF(this.id).subscribe({
      next: (data) => { this.item.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar orden'); this.loading.set(false); },
    });
  }
}
