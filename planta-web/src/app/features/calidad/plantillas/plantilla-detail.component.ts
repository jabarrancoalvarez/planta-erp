import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { CalidadService, PlantillaDetailDto } from '../../../core/services/calidad.service';

@Component({
  selector: 'app-plantilla-detail',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="detail-page">
      <div class="detail-page__header">
        <button class="detail-page__back" (click)="goBack()">&larr; Volver</button>
        <h1 class="detail-page__title">Detalle de Plantilla</h1>
        @if (item()) {
          <button class="detail-page__back" style="margin-left:auto;background:#fee;color:#c00;" (click)="onDelete()">Eliminar</button>
        }
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando plantilla...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else if (item(); as p) {
        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Informacion General</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Nombre</span>
              <span class="detail-page__field-value">{{ p.nombre }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Origen</span>
              <span class="detail-page__field-value">{{ p.origenInspeccion }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Version</span>
              <span class="detail-page__field-value"><code>v{{ p.version }}</code></span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Estado</span>
              <span class="detail-page__field-value">
                <span class="active-dot" [class.active-dot--active]="p.activa" [class.active-dot--inactive]="!p.activa"></span>
                {{ p.activa ? 'Activa' : 'Inactiva' }}
              </span>
            </div>
          </div>
        </div>

        @if (p.descripcion) {
          <div class="detail-page__section">
            <h2 class="detail-page__section-title">Descripcion</h2>
            <p>{{ p.descripcion }}</p>
          </div>
        }

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Criterios ({{ p.criterios.length }})</h2>
          @if (p.criterios.length > 0) {
            <div class="table-container">
              <table class="data-table">
                <thead>
                  <tr>
                    <th>Nombre</th>
                    <th>Min</th>
                    <th>Max</th>
                    <th>Unidad</th>
                    <th>Obligatorio</th>
                  </tr>
                </thead>
                <tbody>
                  @for (c of p.criterios; track c.id) {
                    <tr>
                      <td>{{ c.nombre }}</td>
                      <td>{{ c.valorMinimo ?? '---' }}</td>
                      <td>{{ c.valorMaximo ?? '---' }}</td>
                      <td>{{ c.unidadMedida ?? '---' }}</td>
                      <td>
                        <span class="badge" [class.badge--success]="c.obligatorio" [class.badge--neutral]="!c.obligatorio">
                          {{ c.obligatorio ? 'Si' : 'No' }}
                        </span>
                      </td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          } @else {
            <p class="empty-state">Sin criterios definidos</p>
          }
        </div>
      }
    </div>
  `,
})
export class PlantillaDetailComponent implements OnInit {
  private svc = inject(CalidadService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  readonly item = signal<PlantillaDetailDto | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.loading.set(true);
    this.svc.getPlantilla(id).subscribe({
      next: (data) => { this.item.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar plantilla'); this.loading.set(false); },
    });
  }

  goBack(): void { this.router.navigate(['/app/calidad/plantillas']); }

  onDelete(): void {
    const p = this.item();
    if (!p) return;
    if (!confirm(`¿Eliminar plantilla "${p.nombre}"?`)) return;
    this.svc.deletePlantilla(p.id).subscribe({
      next: () => this.router.navigate(['/app/calidad/plantillas']),
      error: (err) => this.error.set(err?.error?.message ?? 'Error al eliminar'),
    });
  }
}
