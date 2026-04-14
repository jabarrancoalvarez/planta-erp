import { Component, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { IncidenciasService, TipoIncidencia, SeveridadIncidencia } from '../../core/services/incidencias.service';
import { ActivosService, ActivoListDto } from '../../core/services/activos.service';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../shared/components/toast/notification.service';

@Component({
  selector: 'app-incidencia-form',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="movil-page">
      <header class="movil-page__header">
        <a class="back" routerLink="..">←</a>
        <h1>Nueva Incidencia</h1>
      </header>

      <form [formGroup]="form" (ngSubmit)="onSubmit()" class="movil-form">
        <label>Título *</label>
        <input formControlName="titulo" placeholder="Ej: Cinta parada" />

        <label>Tipo *</label>
        <select formControlName="tipo">
          <option value="Averia">Avería</option>
          <option value="Parada">Parada de línea</option>
          <option value="Seguridad">Seguridad</option>
          <option value="CasiAccidente">Casi-accidente</option>
          <option value="Calidad">Calidad</option>
          <option value="Otro">Otro</option>
        </select>

        <label>Severidad *</label>
        <select formControlName="severidad">
          <option value="Baja">Baja</option>
          <option value="Media">Media</option>
          <option value="Alta">Alta</option>
          <option value="Critica">Crítica</option>
        </select>

        <label>Activo afectado</label>
        <select formControlName="activoId">
          <option [ngValue]="null">-- sin activo --</option>
          @for (a of activos(); track a.id) {
            <option [ngValue]="a.id">{{ a.codigo }} — {{ a.nombre }}</option>
          }
        </select>

        <label>Ubicación</label>
        <input formControlName="ubicacionTexto" placeholder="Nave 2 — Línea A" />

        <label>Descripción *</label>
        <textarea formControlName="descripcion" rows="4" placeholder="Qué ha pasado..."></textarea>

        <label class="checkbox">
          <input type="checkbox" formControlName="paradaLinea" />
          Ha parado la línea
        </label>

        <label>Foto</label>
        <input type="file" accept="image/*" capture="environment" (change)="onFile($event)" />
        @if (fotoDataUrl()) {
          <img [src]="fotoDataUrl()" alt="foto" class="preview" />
        }

        <div class="actions">
          <button type="submit" class="btn-primary" [disabled]="form.invalid || submitting()">
            {{ submitting() ? 'Enviando...' : 'Reportar Incidencia' }}
          </button>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .movil-page { padding: 1rem; min-height: 100vh; background: #f5f5f7; }
    .movil-page__header { display: flex; align-items: center; gap: 0.75rem; margin-bottom: 1rem; }
    .movil-page__header h1 { margin: 0; font-size: 1.25rem; }
    .back { text-decoration: none; font-size: 1.5rem; color: #111; padding: 0.25rem 0.75rem; background: white; border-radius: 8px; }
    .movil-form { display: flex; flex-direction: column; gap: 0.5rem; background: white; padding: 1rem; border-radius: 12px; }
    .movil-form label { font-weight: 600; font-size: 0.9rem; margin-top: 0.5rem; }
    .movil-form input, .movil-form select, .movil-form textarea {
      padding: 0.75rem; border: 1px solid #ddd; border-radius: 8px; font-size: 1rem; width: 100%;
    }
    .checkbox { display: flex; align-items: center; gap: 0.5rem; }
    .checkbox input { width: auto; }
    .preview { max-width: 100%; border-radius: 8px; margin-top: 0.5rem; }
    .actions { margin-top: 1rem; }
    .btn-primary { width: 100%; padding: 1rem; background: #dc2626; color: white; border: none; border-radius: 12px; font-size: 1.05rem; font-weight: 600; }
    .btn-primary:disabled { opacity: 0.5; }
  `],
})
export class IncidenciaFormComponent {
  private fb = inject(FormBuilder);
  private svc = inject(IncidenciasService);
  private activosSvc = inject(ActivosService);
  private auth = inject(AuthService);
  private router = inject(Router);
  private notify = inject(NotificationService);

  readonly submitting = signal(false);
  readonly activos = signal<ActivoListDto[]>([]);
  readonly fotoDataUrl = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    titulo: ['', Validators.required],
    tipo: ['Averia' as TipoIncidencia, Validators.required],
    severidad: ['Media' as SeveridadIncidencia, Validators.required],
    activoId: [null as string | null],
    ubicacionTexto: [''],
    descripcion: ['', Validators.required],
    paradaLinea: [false],
  });

  constructor() {
    this.activosSvc.listActivos({ estado: 'Operativo', pageSize: 100 }).subscribe({
      next: (res) => this.activos.set(res.items),
      error: () => this.activos.set([]),
    });
  }

  onFile(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;
    const reader = new FileReader();
    reader.onload = () => this.fotoDataUrl.set(reader.result as string);
    reader.readAsDataURL(file);
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    const user = this.auth.currentUser();
    if (!user) {
      this.notify.error('Sesión expirada');
      return;
    }
    this.submitting.set(true);
    const v = this.form.getRawValue();
    const codigo = `INC-${Date.now().toString().slice(-8)}`;
    this.svc.createIncidencia({
      codigo,
      titulo: v.titulo,
      descripcion: v.descripcion,
      tipo: v.tipo,
      severidad: v.severidad,
      reportadoPorUserId: user.id,
      activoId: v.activoId ?? undefined,
      ubicacionTexto: v.ubicacionTexto || undefined,
      paradaLinea: v.paradaLinea,
      fotosUrl: this.fotoDataUrl() ? [this.fotoDataUrl()!] : undefined,
    }).subscribe({
      next: () => {
        this.notify.success('Incidencia reportada');
        this.router.navigate(['/app/movil']);
      },
      error: (err) => {
        this.notify.error(err?.error?.message ?? 'Error al reportar');
        this.submitting.set(false);
      },
    });
  }
}
