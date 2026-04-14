import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RrhhService, AusenciaDto, TipoAusencia, EstadoAusencia, EmpleadoListDto } from '../../core/services/rrhh.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-ausencias-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule, DatePipe],
  template: `
    <div class="list-page">
      <div class="list-page__header">
        <h1 class="list-page__title">Ausencias</h1>
        <button class="btn-primary" (click)="toggleCreate()">{{ showCreate() ? 'Cancelar' : '+ Nueva ausencia' }}</button>
      </div>

      @if (showCreate()) {
        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Crear ausencia</h2>
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
              <label class="detail-page__field-label">Desde *</label>
              <input type="date" [(ngModel)]="newDesde" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Hasta *</label>
              <input type="date" [(ngModel)]="newHasta" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Motivo</label>
              <input [(ngModel)]="newMotivo" />
            </div>
          </div>
          @if (createError()) { <div class="error-state">{{ createError() }}</div> }
          <div style="display:flex; gap:0.5rem; margin-top:1rem;">
            <button class="btn-primary" (click)="save()" [disabled]="saving()">{{ saving() ? 'Creando...' : 'Crear' }}</button>
          </div>
        </div>
      }

      <div class="filters-bar">
        <select [(ngModel)]="filterEstado" (ngModelChange)="load()">
          <option value="">Todos los estados</option>
          @for (e of estados; track e) { <option [value]="e">{{ e }}</option> }
        </select>
      </div>

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
                <th>Desde</th>
                <th>Hasta</th>
                <th>Dias</th>
                <th>Estado</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              @for (a of items(); track a.id) {
                @if (editingId() === a.id) {
                  <tr>
                    <td>{{ a.empleadoNombre }}</td>
                    <td>
                      <select [(ngModel)]="editTipo">
                        @for (t of tipos; track t) { <option [value]="t">{{ t }}</option> }
                      </select>
                    </td>
                    <td><input type="date" [(ngModel)]="editDesde" /></td>
                    <td><input type="date" [(ngModel)]="editHasta" /></td>
                    <td>---</td>
                    <td>{{ a.estado }}</td>
                    <td>
                      <button class="btn-primary btn-sm" (click)="saveEdit(a.id)" [disabled]="savingEdit()">Guardar</button>
                      <button class="btn-outline btn-sm" (click)="cancelEdit()">Cancelar</button>
                    </td>
                  </tr>
                } @else {
                  <tr>
                    <td>{{ a.empleadoNombre }}</td>
                    <td>{{ a.tipo }}</td>
                    <td>{{ a.fechaInicio | date:'dd/MM/yyyy' }}</td>
                    <td>{{ a.fechaFin | date:'dd/MM/yyyy' }}</td>
                    <td>{{ a.diasTotales }}</td>
                    <td><span class="badge">{{ a.estado }}</span></td>
                    <td>
                      @if (a.estado === 'Solicitada') {
                        <button class="btn-outline btn-sm" (click)="aprobar(a)">Aprobar</button>
                        <button class="btn-outline btn-sm" (click)="rechazar(a)">Rechazar</button>
                        <button class="btn-outline btn-sm" (click)="startEdit(a)">Editar</button>
                      }
                      <button class="btn-outline btn-sm" style="background:#fee;color:#c00;" (click)="remove(a)">Eliminar</button>
                    </td>
                  </tr>
                }
              } @empty {
                <tr><td colspan="7" class="empty-state">Sin ausencias</td></tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `,
})
export class AusenciasListComponent implements OnInit {
  private svc = inject(RrhhService);
  private auth = inject(AuthService);

  readonly items = signal<AusenciaDto[]>([]);
  readonly empleados = signal<EmpleadoListDto[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly showCreate = signal(false);
  readonly saving = signal(false);
  readonly createError = signal<string | null>(null);

  tipos: TipoAusencia[] = ['Vacaciones', 'BajaMedica', 'AsuntoPropio', 'Formacion', 'Maternidad', 'Paternidad', 'Otro'];
  estados: EstadoAusencia[] = ['Solicitada', 'Aprobada', 'Rechazada', 'EnDisfrute', 'Finalizada'];

  filterEstado = '';

  newEmpleadoId = '';
  newTipo: TipoAusencia = 'Vacaciones';
  newDesde = '';
  newHasta = '';
  newMotivo = '';

  readonly editingId = signal<string | null>(null);
  readonly savingEdit = signal(false);
  editTipo: TipoAusencia = 'Vacaciones';
  editDesde = '';
  editHasta = '';
  private editMotivo: string | null = null;

  startEdit(a: AusenciaDto): void {
    this.editingId.set(a.id);
    this.editTipo = a.tipo;
    this.editDesde = new Date(a.fechaInicio).toISOString().slice(0, 10);
    this.editHasta = new Date(a.fechaFin).toISOString().slice(0, 10);
    this.editMotivo = a.motivo;
  }
  cancelEdit(): void { this.editingId.set(null); }
  saveEdit(id: string): void {
    this.savingEdit.set(true);
    this.svc.updateAusencia(id, {
      tipo: this.editTipo,
      fechaInicio: new Date(this.editDesde).toISOString(),
      fechaFin: new Date(this.editHasta).toISOString(),
      motivo: this.editMotivo,
    }).subscribe({
      next: () => { this.savingEdit.set(false); this.editingId.set(null); this.load(); },
      error: (err) => { this.savingEdit.set(false); this.error.set(err?.error?.message ?? 'Error al actualizar'); },
    });
  }
  remove(a: AusenciaDto): void {
    if (!confirm(`¿Eliminar ausencia de "${a.empleadoNombre}"?`)) return;
    this.svc.deleteAusencia(a.id).subscribe({
      next: () => this.load(),
      error: (err) => this.error.set(err?.error?.message ?? 'Error al eliminar'),
    });
  }

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
    this.svc.listAusencias({ estado: (this.filterEstado as EstadoAusencia) || undefined }).subscribe({
      next: (r) => { this.items.set(r.items); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar'); this.loading.set(false); },
    });
  }

  toggleCreate(): void {
    this.showCreate.update(v => !v);
    this.createError.set(null);
  }

  save(): void {
    if (!this.newEmpleadoId || !this.newDesde || !this.newHasta) {
      this.createError.set('Empleado, desde y hasta obligatorios');
      return;
    }
    this.saving.set(true);
    this.svc.createAusencia({
      empleadoId: this.newEmpleadoId,
      tipo: this.newTipo,
      fechaInicio: this.newDesde,
      fechaFin: this.newHasta,
      motivo: this.newMotivo || undefined,
    }).subscribe({
      next: () => {
        this.saving.set(false);
        this.showCreate.set(false);
        this.newDesde = ''; this.newHasta = ''; this.newMotivo = '';
        this.load();
      },
      error: (err) => {
        this.createError.set(err?.error?.message ?? 'Error al crear');
        this.saving.set(false);
      },
    });
  }

  private currentUserId(): string {
    return this.auth.currentUser()?.id ?? '00000000-0000-0000-0000-000000000000';
  }

  aprobar(a: AusenciaDto): void {
    this.svc.aprobarAusencia(a.id, this.currentUserId()).subscribe({
      next: () => this.load(),
      error: (e) => this.error.set(e?.error?.message ?? 'Error aprobar'),
    });
  }

  rechazar(a: AusenciaDto): void {
    this.svc.rechazarAusencia(a.id, this.currentUserId()).subscribe({
      next: () => this.load(),
      error: (e) => this.error.set(e?.error?.message ?? 'Error rechazar'),
    });
  }
}
