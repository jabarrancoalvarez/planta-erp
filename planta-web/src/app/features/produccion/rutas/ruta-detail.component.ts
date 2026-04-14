import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ProduccionService, RutaDetailDto } from '../../../core/services/produccion.service';

@Component({
  selector: 'app-ruta-detail',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="detail-page">
      <div class="detail-page__header">
        <button class="detail-page__back" (click)="goBack()">&larr; Volver</button>
        <h1 class="detail-page__title">Detalle de Ruta</h1>
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
        <div class="loading-state">Cargando ruta...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else if (item(); as r) {
        @if (editing()) {
          <div class="detail-page__section">
            <h2 class="detail-page__section-title">Editar Ruta</h2>
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
    this.svc.getRuta(id).subscribe({
      next: (data) => { this.item.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar ruta'); this.loading.set(false); },
    });
  }

  startEdit(): void {
    const r = this.item();
    if (!r) return;
    this.edNombre = r.nombre;
    this.edDescripcion = r.descripcion ?? '';
    this.saveError.set(null);
    this.editing.set(true);
  }

  cancelEdit(): void {
    this.editing.set(false);
    this.saveError.set(null);
  }

  saveEdit(): void {
    const r = this.item();
    if (!r) return;
    this.saving.set(true);
    this.saveError.set(null);
    this.svc.updateRuta(r.id, {
      nombre: this.edNombre,
      descripcion: this.edDescripcion || undefined,
    }).subscribe({
      next: () => {
        this.saving.set(false);
        this.editing.set(false);
        this.load(r.id);
      },
      error: (err) => {
        this.saveError.set(err?.error?.message ?? 'Error al guardar');
        this.saving.set(false);
      },
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
