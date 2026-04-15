import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

interface UsuarioEmpresa {
  id: string;
  email: string;
  nombre: string;
  rol: string;
  modulosDeshabilitados: string[];
}

const MODULOS = [
  'inventario', 'produccion', 'compras', 'ventas', 'facturacion', 'calidad',
  'activos', 'mantenimiento', 'incidencias', 'crm', 'rrhh', 'costes', 'oee',
  'importador', 'auditoria', 'ia', 'movil',
];

@Component({
  selector: 'app-permisos-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="list-page">
      <div class="list-page__header">
        <h1 class="list-page__title">Permisos por usuario</h1>
      </div>
      <p class="page__subtitle">Activa o desactiva módulos individualmente por usuario (anula el acceso del rol)</p>

      @if (loading()) {
        <div class="loading-state">Cargando usuarios...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Usuario</th>
                <th>Rol</th>
                <th>Módulos deshabilitados</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              @for (u of users(); track u.id) {
                <tr>
                  <td>
                    <div style="font-weight:600">{{ u.nombre }}</div>
                    <div style="font-size:0.85em;color:#666">{{ u.email }}</div>
                  </td>
                  <td><span class="badge">{{ u.rol }}</span></td>
                  <td>
                    @if (editingId() === u.id) {
                      <div style="display:flex;flex-wrap:wrap;gap:0.4rem;">
                        @for (m of modulos; track m) {
                          <label style="display:flex;align-items:center;gap:0.25rem;font-size:0.85em;">
                            <input type="checkbox" [checked]="editing().has(m)" (change)="toggleModule(m, $event)" />
                            {{ m }}
                          </label>
                        }
                      </div>
                    } @else {
                      @if (u.modulosDeshabilitados.length === 0) {
                        <span style="color:#888">— ninguno</span>
                      } @else {
                        @for (m of u.modulosDeshabilitados; track m) {
                          <span class="badge badge--neutral" style="margin-right:0.25rem">{{ m }}</span>
                        }
                      }
                    }
                  </td>
                  <td>
                    @if (editingId() === u.id) {
                      <button class="btn-primary btn-sm" (click)="save(u)" [disabled]="saving()">Guardar</button>
                      <button class="btn-outline btn-sm" (click)="cancel()">Cancelar</button>
                    } @else {
                      <button class="btn-outline btn-sm" (click)="startEdit(u)">Editar</button>
                    }
                  </td>
                </tr>
              } @empty {
                <tr><td colspan="4" class="empty-state">Sin usuarios</td></tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `,
})
export class PermisosListComponent implements OnInit {
  private http = inject(HttpClient);

  readonly modulos = MODULOS;
  readonly users = signal<UsuarioEmpresa[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly editingId = signal<string | null>(null);
  readonly editing = signal<Set<string>>(new Set());
  readonly saving = signal(false);

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.http.get<any[]>(`${environment.apiUrl}/seguridad/usuarios`).subscribe({
      next: (r) => {
        this.users.set(r.map(u => ({
          id: u.id,
          email: u.email,
          nombre: u.nombre,
          rol: u.rol,
          modulosDeshabilitados: u.modulosDeshabilitados ?? [],
        })));
        this.loading.set(false);
      },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar usuarios'); this.loading.set(false); },
    });
  }

  startEdit(u: UsuarioEmpresa): void {
    this.editingId.set(u.id);
    this.editing.set(new Set(u.modulosDeshabilitados));
  }

  cancel(): void { this.editingId.set(null); }

  toggleModule(m: string, event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    const next = new Set(this.editing());
    if (checked) next.add(m); else next.delete(m);
    this.editing.set(next);
  }

  save(u: UsuarioEmpresa): void {
    this.saving.set(true);
    const modulosDeshabilitados = Array.from(this.editing());
    this.http.put(`${environment.apiUrl}/seguridad/usuarios/${u.id}/modulos`, { modulosDeshabilitados }).subscribe({
      next: () => {
        this.saving.set(false);
        this.editingId.set(null);
        this.load();
      },
      error: (err) => {
        this.saving.set(false);
        this.error.set(err?.error?.message ?? 'Error al guardar');
      },
    });
  }
}
