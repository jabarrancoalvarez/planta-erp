import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RrhhService, EmpleadoListDto } from '../../core/services/rrhh.service';

@Component({
  selector: 'app-empleados-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="list-page">
      <div class="list-page__header">
        <h1 class="list-page__title">Empleados</h1>
        <button class="btn-primary" (click)="toggleCreate()">{{ showCreate() ? 'Cancelar' : '+ Nuevo empleado' }}</button>
      </div>

      @if (showCreate()) {
        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Crear empleado</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <label class="detail-page__field-label">Codigo *</label>
              <input [(ngModel)]="newCodigo" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Nombre *</label>
              <input [(ngModel)]="newNombre" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Apellidos *</label>
              <input [(ngModel)]="newApellidos" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">DNI *</label>
              <input [(ngModel)]="newDni" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Puesto *</label>
              <input [(ngModel)]="newPuesto" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Departamento</label>
              <input [(ngModel)]="newDepartamento" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Email</label>
              <input type="email" [(ngModel)]="newEmail" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Coste/hora</label>
              <input type="number" [(ngModel)]="newCoste" />
            </div>
          </div>
          @if (createError()) { <div class="error-state">{{ createError() }}</div> }
          <div style="display:flex; gap:0.5rem; margin-top:1rem;">
            <button class="btn-primary" (click)="save()" [disabled]="saving()">{{ saving() ? 'Creando...' : 'Crear' }}</button>
          </div>
        </div>
      }

      <div class="filters-bar">
        <input [(ngModel)]="filterSearch" (ngModelChange)="load()" placeholder="Buscar..." />
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
                <th>Codigo</th>
                <th>Nombre</th>
                <th>DNI</th>
                <th>Puesto</th>
                <th>Departamento</th>
                <th>Activo</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              @for (e of items(); track e.id) {
                @if (editingId() === e.id) {
                  <tr>
                    <td><code>{{ e.codigo }}</code></td>
                    <td><input [(ngModel)]="editNombre" style="width:6rem" /> <input [(ngModel)]="editApellidos" style="width:6rem" /></td>
                    <td>{{ e.dni }}</td>
                    <td><input [(ngModel)]="editPuesto" /></td>
                    <td><input [(ngModel)]="editDepartamento" /></td>
                    <td>{{ e.activo ? 'Si' : 'No' }}</td>
                    <td>
                      <button class="btn-primary btn-sm" (click)="saveEdit(e.id)" [disabled]="savingEdit()">Guardar</button>
                      <button class="btn-outline btn-sm" (click)="cancelEdit()">Cancelar</button>
                    </td>
                  </tr>
                } @else {
                  <tr>
                    <td><code>{{ e.codigo }}</code></td>
                    <td>{{ e.nombre }} {{ e.apellidos }}</td>
                    <td>{{ e.dni }}</td>
                    <td>{{ e.puesto }}</td>
                    <td>{{ e.departamento ?? '---' }}</td>
                    <td>
                      <span class="badge" [class.badge--success]="e.activo" [class.badge--neutral]="!e.activo">
                        {{ e.activo ? 'Si' : 'No' }}
                      </span>
                    </td>
                    <td>
                      <button class="btn-outline btn-sm" (click)="startEdit(e)">Editar</button>
                      <button class="btn-outline btn-sm" style="background:#fee;color:#c00;" (click)="remove(e)">Eliminar</button>
                    </td>
                  </tr>
                }
              } @empty {
                <tr><td colspan="7" class="empty-state">Sin empleados</td></tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `,
})
export class EmpleadosListComponent implements OnInit {
  private svc = inject(RrhhService);

  readonly items = signal<EmpleadoListDto[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly showCreate = signal(false);
  readonly saving = signal(false);
  readonly createError = signal<string | null>(null);

  filterSearch = '';

  newCodigo = '';
  newNombre = '';
  newApellidos = '';
  newDni = '';
  newPuesto = '';
  newDepartamento = '';
  newEmail = '';
  newCoste = 0;

  readonly editingId = signal<string | null>(null);
  readonly savingEdit = signal(false);
  editNombre = '';
  editApellidos = '';
  editPuesto = '';
  editDepartamento = '';
  private editEmail: string | null = null;
  private editTelefono: string | null = null;
  private editCoste = 0;
  private editDias = 22;

  startEdit(e: EmpleadoListDto): void {
    this.editingId.set(e.id);
    this.editNombre = e.nombre;
    this.editApellidos = e.apellidos;
    this.editPuesto = e.puesto;
    this.editDepartamento = e.departamento ?? '';
    this.editEmail = e.email;
    this.editTelefono = e.telefono;
    this.editCoste = e.costeHoraEstandar;
    this.editDias = e.diasVacacionesAnuales;
  }
  cancelEdit(): void { this.editingId.set(null); }
  saveEdit(id: string): void {
    this.savingEdit.set(true);
    this.svc.updateEmpleado(id, {
      nombre: this.editNombre,
      apellidos: this.editApellidos,
      puesto: this.editPuesto,
      email: this.editEmail,
      telefono: this.editTelefono,
      departamento: this.editDepartamento || null,
      costeHoraEstandar: this.editCoste,
      diasVacacionesAnuales: this.editDias,
    }).subscribe({
      next: () => { this.savingEdit.set(false); this.editingId.set(null); this.load(); },
      error: (err) => { this.savingEdit.set(false); this.error.set(err?.error?.message ?? 'Error al actualizar'); },
    });
  }
  remove(e: EmpleadoListDto): void {
    if (!confirm(`¿Eliminar empleado "${e.nombre} ${e.apellidos}"?`)) return;
    this.svc.deleteEmpleado(e.id).subscribe({
      next: () => this.load(),
      error: (err) => this.error.set(err?.error?.message ?? 'Error al eliminar'),
    });
  }

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.svc.listEmpleados({ search: this.filterSearch || undefined }).subscribe({
      next: (r) => { this.items.set(r.items); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar'); this.loading.set(false); },
    });
  }

  toggleCreate(): void {
    this.showCreate.update(v => !v);
    this.createError.set(null);
  }

  save(): void {
    if (!this.newCodigo || !this.newNombre || !this.newApellidos || !this.newDni || !this.newPuesto) {
      this.createError.set('Codigo, nombre, apellidos, DNI y puesto obligatorios');
      return;
    }
    this.saving.set(true);
    this.svc.createEmpleado({
      codigo: this.newCodigo,
      nombre: this.newNombre,
      apellidos: this.newApellidos,
      dni: this.newDni,
      puesto: this.newPuesto,
      departamento: this.newDepartamento || undefined,
      email: this.newEmail || undefined,
      costeHoraEstandar: this.newCoste,
    }).subscribe({
      next: () => {
        this.saving.set(false);
        this.showCreate.set(false);
        this.newCodigo = ''; this.newNombre = ''; this.newApellidos = '';
        this.newDni = ''; this.newPuesto = ''; this.newDepartamento = '';
        this.newEmail = ''; this.newCoste = 0;
        this.load();
      },
      error: (err) => {
        this.createError.set(err?.error?.message ?? 'Error al crear');
        this.saving.set(false);
      },
    });
  }
}
