import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CalidadService, PlantillaDetailDto } from '../../../core/services/calidad.service';

@Component({
  selector: 'app-plantilla-detail',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="detail-page">
      <div class="detail-page__header">
        <button class="detail-page__back" (click)="goBack()">&larr; Volver</button>
        <h1 class="detail-page__title">Detalle de Plantilla</h1>
        @if (item()) {
          <div style="margin-left:auto; display:flex; gap:0.5rem;">
            @if (!editing()) {
              <button class="detail-page__back" (click)="startEdit()">Editar</button>
            }
            <button class="detail-page__back" style="background:#fee;color:#c00;" (click)="onDelete()">Eliminar</button>
          </div>
        }
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando plantilla...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else if (item(); as p) {
        @if (editing()) {
          <div class="detail-page__section">
            <h2 class="detail-page__section-title">Editar Plantilla</h2>
            <div class="detail-page__grid">
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="ed-nombre">Nombre *</label>
                <input id="ed-nombre" [(ngModel)]="edNombre" />
              </div>
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="ed-desc">Descripción</label>
                <input id="ed-desc" [(ngModel)]="edDescripcion" />
              </div>
            </div>
            @if (saveError()) { <div class="error-state">{{ saveError() }}</div> }
            <div style="display:flex; gap:0.5rem; margin-top:1rem;">
              <button class="detail-page__back" (click)="saveEdit()" [disabled]="saving()">
                {{ saving() ? 'Guardando...' : 'Guardar' }}
              </button>
              <button class="detail-page__back" (click)="cancelEdit()">Cancelar</button>
            </div>
          </div>
        }

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
  readonly editing = signal(false);
  readonly saving = signal(false);
  readonly saveError = signal<string | null>(null);

  edNombre = '';
  edDescripcion = '';

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.load(id);
  }

  private load(id: string): void {
    this.loading.set(true);
    this.svc.getPlantilla(id).subscribe({
      next: (data) => { this.item.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar plantilla'); this.loading.set(false); },
    });
  }

  startEdit(): void {
    const p = this.item();
    if (!p) return;
    this.edNombre = p.nombre;
    this.edDescripcion = p.descripcion ?? '';
    this.saveError.set(null);
    this.editing.set(true);
  }

  cancelEdit(): void {
    this.editing.set(false);
    this.saveError.set(null);
  }

  saveEdit(): void {
    const p = this.item();
    if (!p) return;
    this.saving.set(true);
    this.saveError.set(null);
    this.svc.updatePlantilla(p.id, {
      nombre: this.edNombre,
      descripcion: this.edDescripcion || undefined,
    }).subscribe({
      next: () => {
        this.saving.set(false);
        this.editing.set(false);
        this.load(p.id);
      },
      error: (err) => {
        this.saveError.set(err?.error?.message ?? 'Error al guardar');
        this.saving.set(false);
      },
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
