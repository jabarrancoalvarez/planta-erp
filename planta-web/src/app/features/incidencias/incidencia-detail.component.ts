import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { IncidenciasService, IncidenciaDetailDto } from '../../core/services/incidencias.service';

@Component({
  selector: 'app-incidencia-detail',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="detail-page">
      <div class="detail-page__header">
        <button class="detail-page__back" (click)="goBack()">&larr; Volver</button>
        <h1 class="detail-page__title">Detalle de Incidencia</h1>
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
      } @else if (item(); as i) {
        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Datos Generales</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Código</span>
              <span class="detail-page__field-value"><code>{{ i.codigo }}</code></span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Título</span>
              <span class="detail-page__field-value">{{ i.titulo }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Tipo</span>
              <span class="detail-page__field-value">{{ i.tipo }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Severidad</span>
              <span class="detail-page__field-value">{{ i.severidad }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Estado</span>
              <span class="detail-page__field-value">{{ i.estado }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Parada Línea</span>
              <span class="detail-page__field-value">{{ i.paradaLinea ? 'Sí' : 'No' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Descripción</span>
              <span class="detail-page__field-value">{{ i.descripcion }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Ubicación</span>
              <span class="detail-page__field-value">{{ i.ubicacionTexto ?? '---' }}</span>
            </div>
          </div>
        </div>

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Fechas y Asignación</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Fecha Apertura</span>
              <span class="detail-page__field-value">{{ i.fechaApertura | date:'dd/MM/yyyy HH:mm' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Fecha Cierre</span>
              <span class="detail-page__field-value">{{ (i.fechaCierre | date:'dd/MM/yyyy HH:mm') ?? '---' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Reportado Por</span>
              <span class="detail-page__field-value">{{ i.reportadoPorUserId }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Asignado A</span>
              <span class="detail-page__field-value">{{ i.asignadoAUserId ?? '---' }}</span>
            </div>
          </div>
        </div>

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Resolución</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Causa Raíz</span>
              <span class="detail-page__field-value">{{ i.causaRaiz ?? '---' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Notas Resolución</span>
              <span class="detail-page__field-value">{{ i.resolucionNotas ?? '---' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Orden Trabajo</span>
              <span class="detail-page__field-value">{{ i.ordenTrabajoId ?? '---' }}</span>
            </div>
          </div>
        </div>
      }
    </div>
  `,
})
export class IncidenciaDetailComponent implements OnInit {
  private svc = inject(IncidenciasService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  readonly item = signal<IncidenciaDetailDto | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.loading.set(true);
    this.svc.getIncidencia(id).subscribe({
      next: (data) => { this.item.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar'); this.loading.set(false); },
    });
  }

  goBack(): void { this.router.navigate(['/app/incidencias']); }

  onDelete(): void {
    const i = this.item();
    if (!i) return;
    if (!confirm(`¿Eliminar la incidencia ${i.codigo}?`)) return;
    this.svc.deleteIncidencia(i.id).subscribe({
      next: () => this.router.navigate(['/app/incidencias']),
      error: (err) => this.error.set(err?.error?.message ?? 'Error al eliminar'),
    });
  }
}
