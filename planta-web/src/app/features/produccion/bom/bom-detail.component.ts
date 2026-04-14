import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ProduccionService, BOMDetailDto } from '../../../core/services/produccion.service';

@Component({
  selector: 'app-bom-detail',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="detail-page">
      <div class="detail-page__header">
        <button class="detail-page__back" (click)="goBack()">&larr; Volver</button>
        <h1 class="detail-page__title">Detalle de BOM</h1>
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
        <div class="loading-state">Cargando BOM...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else if (item(); as bom) {
        @if (editing()) {
          <div class="detail-page__section">
            <h2 class="detail-page__section-title">Editar BOM</h2>
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
              <span class="detail-page__field-value">{{ bom.nombre }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Version</span>
              <span class="detail-page__field-value"><code>v{{ bom.versionBOM }}</code></span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Estado</span>
              <span class="detail-page__field-value">
                <span class="active-dot" [class.active-dot--active]="bom.activo" [class.active-dot--inactive]="!bom.activo"></span>
                {{ bom.activo ? 'Activa' : 'Inactiva' }}
              </span>
            </div>
          </div>
        </div>

        @if (bom.descripcion) {
          <div class="detail-page__section">
            <h2 class="detail-page__section-title">Descripcion</h2>
            <p>{{ bom.descripcion }}</p>
          </div>
        }

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Lineas de Material ({{ bom.lineas.length }})</h2>
          @if (bom.lineas.length > 0) {
            <div class="table-container">
              <table class="data-table">
                <thead>
                  <tr>
                    <th>Componente</th>
                    <th>Cantidad</th>
                    <th>Unidad</th>
                    <th>Merma Est.</th>
                  </tr>
                </thead>
                <tbody>
                  @for (line of bom.lineas; track line.id) {
                    <tr>
                      <td>{{ line.componenteNombre }}</td>
                      <td>{{ line.cantidad }}</td>
                      <td>{{ line.unidadMedida }}</td>
                      <td>{{ line.mermaEstimada }}%</td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          } @else {
            <p class="empty-state">Sin lineas de material definidas</p>
          }
        </div>
      }
    </div>
  `,
})
export class BOMDetailComponent implements OnInit {
  private svc = inject(ProduccionService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  readonly item = signal<BOMDetailDto | null>(null);
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
    this.svc.getBOM(id).subscribe({
      next: (data) => { this.item.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar BOM'); this.loading.set(false); },
    });
  }

  startEdit(): void {
    const bom = this.item();
    if (!bom) return;
    this.edNombre = bom.nombre;
    this.edDescripcion = bom.descripcion ?? '';
    this.saveError.set(null);
    this.editing.set(true);
  }

  cancelEdit(): void {
    this.editing.set(false);
    this.saveError.set(null);
  }

  saveEdit(): void {
    const bom = this.item();
    if (!bom) return;
    this.saving.set(true);
    this.saveError.set(null);
    this.svc.updateBOM(bom.id, {
      nombre: this.edNombre,
      descripcion: this.edDescripcion || undefined,
    }).subscribe({
      next: () => {
        this.saving.set(false);
        this.editing.set(false);
        this.load(bom.id);
      },
      error: (err) => {
        this.saveError.set(err?.error?.message ?? 'Error al guardar');
        this.saving.set(false);
      },
    });
  }

  goBack(): void { this.router.navigate(['/app/produccion/bom']); }

  onDelete(): void {
    const bom = this.item();
    if (!bom) return;
    if (!confirm(`¿Eliminar BOM "${bom.nombre}"?`)) return;
    this.svc.deleteBOM(bom.id).subscribe({
      next: () => this.router.navigate(['/app/produccion/bom']),
      error: (err) => this.error.set(err?.error?.message ?? 'Error al eliminar'),
    });
  }
}
