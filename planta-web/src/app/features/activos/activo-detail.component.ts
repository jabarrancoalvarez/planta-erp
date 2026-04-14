import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ActivosService, ActivoDto, EstadoActivo, CriticidadActivo } from '../../core/services/activos.service';

@Component({
  selector: 'app-activo-detail',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="detail-page">
      <div class="detail-page__header">
        <button class="detail-page__back" (click)="goBack()">&larr; Volver</button>
        <h1 class="detail-page__title">Detalle de Activo</h1>
        @if (item()) {
          <div style="margin-left:auto; display:flex; gap:0.5rem;">
            @if (!editing()) {
              <button class="detail-page__back" (click)="startEdit()">Editar</button>
            }
            <button class="detail-page__back" (click)="startChangeEstado()">Cambiar Estado</button>
            <button class="detail-page__back" style="background:#fee;color:#c00;" (click)="onDelete()">Eliminar</button>
          </div>
        }
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando activo...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else if (item(); as a) {
        @if (editing()) {
          <div class="detail-page__section">
            <h2 class="detail-page__section-title">Editar Activo</h2>
            <div class="detail-page__grid">
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="ed-nombre">Nombre *</label>
                <input id="ed-nombre" [(ngModel)]="edNombre" />
              </div>
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="ed-crit">Criticidad</label>
                <select id="ed-crit" [(ngModel)]="edCriticidad">
                  <option value="Baja">Baja</option>
                  <option value="Media">Media</option>
                  <option value="Alta">Alta</option>
                  <option value="Critica">Crítica</option>
                </select>
              </div>
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="ed-desc">Descripción</label>
                <input id="ed-desc" [(ngModel)]="edDescripcion" />
              </div>
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="ed-ubi">Ubicación</label>
                <input id="ed-ubi" [(ngModel)]="edUbicacion" />
              </div>
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="ed-fab">Fabricante</label>
                <input id="ed-fab" [(ngModel)]="edFabricante" />
              </div>
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="ed-mod">Modelo</label>
                <input id="ed-mod" [(ngModel)]="edModelo" />
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

        @if (changingEstado()) {
          <div class="detail-page__section">
            <h2 class="detail-page__section-title">Cambiar Estado</h2>
            <div class="detail-page__field">
              <label class="detail-page__field-label" for="ed-estado">Nuevo Estado</label>
              <select id="ed-estado" [(ngModel)]="edEstado">
                <option value="Operativo">Operativo</option>
                <option value="EnMantenimiento">En Mantenimiento</option>
                <option value="Averiado">Averiado</option>
                <option value="Parado">Parado</option>
                <option value="Baja">Baja</option>
              </select>
            </div>
            @if (saveError()) {
              <div class="error-state">{{ saveError() }}</div>
            }
            <div style="display:flex; gap:0.5rem; margin-top:1rem;">
              <button class="detail-page__back" (click)="saveEstado()" [disabled]="saving()">
                {{ saving() ? 'Guardando...' : 'Guardar' }}
              </button>
              <button class="detail-page__back" (click)="cancelChangeEstado()">Cancelar</button>
            </div>
          </div>
        }

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Datos Generales</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Código</span>
              <span class="detail-page__field-value"><code>{{ a.codigo }}</code></span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Nombre</span>
              <span class="detail-page__field-value">{{ a.nombre }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Tipo</span>
              <span class="detail-page__field-value">{{ a.tipo }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Criticidad</span>
              <span class="detail-page__field-value">{{ a.criticidad }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Estado</span>
              <span class="detail-page__field-value">{{ a.estado }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Descripción</span>
              <span class="detail-page__field-value">{{ a.descripcion ?? '---' }}</span>
            </div>
          </div>
        </div>

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Ubicación y Fabricante</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Ubicación</span>
              <span class="detail-page__field-value">{{ a.ubicacion ?? '---' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Fabricante</span>
              <span class="detail-page__field-value">{{ a.fabricante ?? '---' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Modelo</span>
              <span class="detail-page__field-value">{{ a.modelo ?? '---' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Número de Serie</span>
              <span class="detail-page__field-value">{{ a.numeroSerie ?? '---' }}</span>
            </div>
          </div>
        </div>

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Datos Económicos</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Fecha Adquisición</span>
              <span class="detail-page__field-value">{{ a.fechaAdquisicion ?? '---' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Coste Adquisición</span>
              <span class="detail-page__field-value">{{ a.costeAdquisicion }} €</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Horas de Uso</span>
              <span class="detail-page__field-value">{{ a.horasUso }} h</span>
            </div>
          </div>
        </div>
      }
    </div>
  `,
})
export class ActivoDetailComponent implements OnInit {
  private svc = inject(ActivosService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  readonly item = signal<ActivoDto | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly changingEstado = signal(false);
  readonly editing = signal(false);
  readonly saving = signal(false);
  readonly saveError = signal<string | null>(null);

  edEstado: EstadoActivo = 'Operativo';
  edNombre = '';
  edCriticidad: CriticidadActivo = 'Media';
  edDescripcion = '';
  edUbicacion = '';
  edFabricante = '';
  edModelo = '';

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.load(id);
  }

  private load(id: string): void {
    this.loading.set(true);
    this.svc.getActivo(id).subscribe({
      next: (data) => { this.item.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar activo'); this.loading.set(false); },
    });
  }

  startEdit(): void {
    const a = this.item();
    if (!a) return;
    this.edNombre = a.nombre;
    this.edCriticidad = a.criticidad;
    this.edDescripcion = a.descripcion ?? '';
    this.edUbicacion = a.ubicacion ?? '';
    this.edFabricante = a.fabricante ?? '';
    this.edModelo = a.modelo ?? '';
    this.saveError.set(null);
    this.editing.set(true);
  }

  cancelEdit(): void {
    this.editing.set(false);
    this.saveError.set(null);
  }

  saveEdit(): void {
    const a = this.item();
    if (!a) return;
    this.saving.set(true);
    this.saveError.set(null);
    this.svc.updateActivo(a.id, {
      nombre: this.edNombre,
      criticidad: this.edCriticidad,
      descripcion: this.edDescripcion || undefined,
      ubicacion: this.edUbicacion || undefined,
      fabricante: this.edFabricante || undefined,
      modelo: this.edModelo || undefined,
    }).subscribe({
      next: () => {
        this.saving.set(false);
        this.editing.set(false);
        this.load(a.id);
      },
      error: (err) => {
        this.saveError.set(err?.error?.message ?? 'Error al guardar');
        this.saving.set(false);
      },
    });
  }

  startChangeEstado(): void {
    const a = this.item();
    if (!a) return;
    this.edEstado = a.estado;
    this.saveError.set(null);
    this.changingEstado.set(true);
  }

  cancelChangeEstado(): void {
    this.changingEstado.set(false);
    this.saveError.set(null);
  }

  saveEstado(): void {
    const a = this.item();
    if (!a) return;
    this.saving.set(true);
    this.saveError.set(null);
    this.svc.cambiarEstado(a.id, this.edEstado).subscribe({
      next: () => {
        this.saving.set(false);
        this.changingEstado.set(false);
        this.load(a.id);
      },
      error: (err) => {
        this.saveError.set(err?.error?.message ?? 'Error al cambiar estado');
        this.saving.set(false);
      },
    });
  }

  goBack(): void { this.router.navigate(['/app/activos']); }

  onDelete(): void {
    const a = this.item();
    if (!a) return;
    if (!confirm(`¿Eliminar activo "${a.nombre}"?`)) return;
    this.svc.deleteActivo(a.id).subscribe({
      next: () => this.router.navigate(['/app/activos']),
      error: (err) => this.error.set(err?.error?.message ?? 'Error al eliminar'),
    });
  }
}
