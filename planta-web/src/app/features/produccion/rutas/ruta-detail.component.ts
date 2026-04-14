import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ProduccionService, RutaDetailDto } from '../../../core/services/produccion.service';

@Component({
  selector: 'app-ruta-detail',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="detail-page">
      <div class="detail-page__header">
        <button class="detail-page__back" (click)="goBack()">&larr; Volver</button>
        <h1 class="detail-page__title">Detalle de Ruta</h1>
        @if (item()) {
          <button class="detail-page__back" style="margin-left:auto;background:#fee;color:#c00;" (click)="onDelete()">Eliminar</button>
        }
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando ruta...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else if (item(); as r) {
        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Información General</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Nombre</span>
              <span class="detail-page__field-value">{{ r.nombre }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Estado</span>
              <span class="detail-page__field-value">
                <span class="active-dot" [class.active-dot--active]="r.activa" [class.active-dot--inactive]="!r.activa"></span>
                {{ r.activa ? 'Activa' : 'Inactiva' }}
              </span>
            </div>
          </div>
        </div>

        @if (r.descripcion) {
          <div class="detail-page__section">
            <h2 class="detail-page__section-title">Descripción</h2>
            <p>{{ r.descripcion }}</p>
          </div>
        }

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Operaciones ({{ r.operaciones.length }})</h2>
          @if (r.operaciones.length > 0) {
            <div class="table-container">
              <table class="data-table">
                <thead>
                  <tr>
                    <th>Nº</th>
                    <th>Nombre</th>
                    <th>Tipo</th>
                    <th>Tiempo (min)</th>
                    <th>Centro Trabajo</th>
                  </tr>
                </thead>
                <tbody>
                  @for (op of r.operaciones; track op.id) {
                    <tr>
                      <td>{{ op.numero }}</td>
                      <td>{{ op.nombre }}</td>
                      <td>{{ op.tipoOperacion }}</td>
                      <td>{{ op.tiempoEstimadoMinutos }}</td>
                      <td>{{ op.centroTrabajo }}</td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          } @else {
            <div class="empty-state">Sin operaciones definidas</div>
          }
        </div>
      }
    </div>
  `,
})
export class RutaDetailComponent implements OnInit {
  private svc = inject(ProduccionService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  readonly item = signal<RutaDetailDto | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.loading.set(true);
    this.svc.getRuta(id).subscribe({
      next: (data) => { this.item.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar ruta'); this.loading.set(false); },
    });
  }

  goBack(): void { this.router.navigate(['/app/produccion/rutas']); }

  onDelete(): void {
    const r = this.item();
    if (!r) return;
    if (!confirm(`¿Eliminar ruta "${r.nombre}"?`)) return;
    this.svc.deleteRuta(r.id).subscribe({
      next: () => this.router.navigate(['/app/produccion/rutas']),
      error: (err) => this.error.set(err?.error?.message ?? 'Error al eliminar'),
    });
  }
}
