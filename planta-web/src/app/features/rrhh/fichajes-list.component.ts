import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RrhhService, FichajeDto, TipoFichaje, EmpleadoListDto } from '../../core/services/rrhh.service';

@Component({
  selector: 'app-fichajes-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule, DatePipe],
  template: `
    <div class="list-page">
      <div class="list-page__header">
        <h1 class="list-page__title">Fichajes</h1>
        <button class="btn-primary" (click)="toggleCreate()">{{ showCreate() ? 'Cancelar' : '+ Registrar fichaje' }}</button>
      </div>

      @if (showCreate()) {
        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Registrar fichaje</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <label class="detail-page__field-label">Empleado *</label>
              <select [(ngModel)]="newEmpleadoId">
                <option value="">--</option>
                @for (e of empleados(); track e.id) {
                  <option [value]="e.id">{{ e.nombre }} {{ e.apellidos }}</option>
                }
              </select>
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Tipo *</label>
              <select [(ngModel)]="newTipo">
                @for (t of tipos; track t) { <option [value]="t">{{ t }}</option> }
              </select>
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Notas</label>
              <input [(ngModel)]="newNotas" />
            </div>
          </div>
          @if (createError()) { <div class="error-state">{{ createError() }}</div> }
          <div style="display:flex; gap:0.5rem; margin-top:1rem;">
            <button class="btn-primary" (click)="save()" [disabled]="saving()">{{ saving() ? 'Guardando...' : 'Registrar' }}</button>
          </div>
        </div>
      }

      @if (loading()) {
        <div class="loading-state">Cargando...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Empleado</th>
                <th>Tipo</th>
                <th>Momento</th>
                <th>Notas</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              @for (f of items(); track f.id) {
                <tr>
                  <td>{{ f.empleadoNombre }}</td>
                  <td><span class="badge">{{ f.tipo }}</span></td>
                  <td>{{ f.momento | date:'dd/MM/yyyy HH:mm' }}</td>
                  <td>{{ f.notas ?? '---' }}</td>
                  <td>
                    <button class="btn-outline btn-sm" style="background:#fee;color:#c00;" (click)="remove(f)">Eliminar</button>
                  </td>
                </tr>
              } @empty {
                <tr><td colspan="5" class="empty-state">Sin fichajes</td></tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `,
})
export class FichajesListComponent implements OnInit {
  private svc = inject(RrhhService);

  readonly items = signal<FichajeDto[]>([]);
  readonly empleados = signal<EmpleadoListDto[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly showCreate = signal(false);
  readonly saving = signal(false);
  readonly createError = signal<string | null>(null);

  tipos: TipoFichaje[] = ['EntradaJornada', 'SalidaJornada', 'InicioDescanso', 'FinDescanso', 'InicioMaquina', 'FinMaquina', 'InicioOF', 'FinOF'];

  newEmpleadoId = '';
  newTipo: TipoFichaje = 'EntradaJornada';
  newNotas = '';

  ngOnInit(): void {
    this.load();
    this.svc.listEmpleados({ pageSize: 100 }).subscribe({
      next: (r) => this.empleados.set(r.items),
      error: () => {},
    });
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.svc.listFichajes().subscribe({
      next: (r) => { this.items.set(r.items); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar'); this.loading.set(false); },
    });
  }

  toggleCreate(): void {
    this.showCreate.update(v => !v);
    this.createError.set(null);
  }

  remove(f: FichajeDto): void {
    if (!confirm(`¿Eliminar fichaje de "${f.empleadoNombre}"?`)) return;
    this.svc.deleteFichaje(f.id).subscribe({
      next: () => this.load(),
      error: (err) => this.error.set(err?.error?.message ?? 'Error al eliminar'),
    });
  }

  save(): void {
    if (!this.newEmpleadoId) {
      this.createError.set('Empleado obligatorio');
      return;
    }
    this.saving.set(true);
    this.svc.registrarFichaje({
      empleadoId: this.newEmpleadoId,
      tipo: this.newTipo,
      notas: this.newNotas || undefined,
    }).subscribe({
      next: () => {
        this.saving.set(false);
        this.showCreate.set(false);
        this.newNotas = '';
        this.load();
      },
      error: (err) => {
        this.createError.set(err?.error?.message ?? 'Error al registrar');
        this.saving.set(false);
      },
    });
  }
}
