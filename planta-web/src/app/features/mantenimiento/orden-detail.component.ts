import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MantenimientoService, OrdenTrabajoDetailDto } from '../../core/services/mantenimiento.service';

@Component({
  selector: 'app-orden-detail',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="detail-page">
      <div class="detail-page__header">
        <button class="detail-page__back" (click)="goBack()">&larr; Volver</button>
        <h1 class="detail-page__title">Detalle de Orden de Trabajo</h1>
        @if (item()) {
          <div style="margin-left:auto;">
            <button class="detail-page__back" style="background:#fee;color:#c00;" (click)="onDelete()">Eliminar</button>
          </div>
        }
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else if (item(); as o) {
        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Datos Generales</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Código</span>
              <span class="detail-page__field-value"><code>{{ o.codigo }}</code></span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Título</span>
              <span class="detail-page__field-value">{{ o.titulo }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Tipo</span>
              <span class="detail-page__field-value">{{ o.tipo }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Estado</span>
              <span class="detail-page__field-value">{{ o.estado }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Prioridad</span>
              <span class="detail-page__field-value">{{ o.prioridad }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Descripción</span>
              <span class="detail-page__field-value">{{ o.descripcion ?? '---' }}</span>
            </div>
          </div>
        </div>

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Planificación y Ejecución</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Fecha Planificada</span>
              <span class="detail-page__field-value">{{ (o.fechaPlanificada | date:'dd/MM/yyyy') ?? '---' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Fecha Inicio</span>
              <span class="detail-page__field-value">{{ (o.fechaInicio | date:'dd/MM/yyyy HH:mm') ?? '---' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Fecha Fin</span>
              <span class="detail-page__field-value">{{ (o.fechaFin | date:'dd/MM/yyyy HH:mm') ?? '---' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Horas Estimadas</span>
              <span class="detail-page__field-value">{{ o.horasEstimadas }} h</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Horas Reales</span>
              <span class="detail-page__field-value">{{ o.horasReales }} h</span>
            </div>
          </div>
        </div>

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Costes y Cierre</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Coste Mano Obra</span>
              <span class="detail-page__field-value">{{ o.costeManoObra }} €</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Coste Repuestos</span>
              <span class="detail-page__field-value">{{ o.costeRepuestos }} €</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Notas Cierre</span>
              <span class="detail-page__field-value">{{ o.notasCierre ?? '---' }}</span>
            </div>
          </div>
        </div>
      }
    </div>
  `,
})
export class OrdenDetailComponent implements OnInit {
  private svc = inject(MantenimientoService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  readonly item = signal<OrdenTrabajoDetailDto | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.loading.set(true);
    this.svc.getOrden(id).subscribe({
      next: (data) => { this.item.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar'); this.loading.set(false); },
    });
  }

  goBack(): void { this.router.navigate(['/app/mantenimiento/ordenes']); }

  onDelete(): void {
    const o = this.item();
    if (!o) return;
    if (!confirm(`¿Eliminar la orden ${o.codigo}?`)) return;
    this.svc.deleteOrden(o.id).subscribe({
      next: () => this.router.navigate(['/app/mantenimiento/ordenes']),
      error: (err) => this.error.set(err?.error?.message ?? 'Error al eliminar'),
    });
  }
}
