import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CrmService, LeadListDto, EstadoLead, OrigenLead } from '../../core/services/crm.service';
import { ExportService } from '../../core/services/export.service';

@Component({
  selector: 'app-leads-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="list-page">
      <div class="list-page__header">
        <h1 class="list-page__title">Leads CRM</h1>
        <div style="display:flex; gap:0.5rem;">
          <button class="btn-outline" (click)="exportCsv()">Exportar CSV</button>
          <button class="btn-primary" (click)="toggleCreate()">{{ showCreate() ? 'Cancelar' : '+ Nuevo lead' }}</button>
        </div>
      </div>

      @if (showCreate()) {
        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Crear lead</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <label class="detail-page__field-label">Nombre *</label>
              <input [(ngModel)]="newNombre" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Origen *</label>
              <select [(ngModel)]="newOrigen">
                @for (o of origenes; track o) { <option [value]="o">{{ o }}</option> }
              </select>
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Empresa</label>
              <input [(ngModel)]="newEmpresa" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Email</label>
              <input [(ngModel)]="newEmail" type="email" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Telefono</label>
              <input [(ngModel)]="newTelefono" />
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
                <th>Nombre</th>
                <th>Empresa</th>
                <th>Email</th>
                <th>Telefono</th>
                <th>Origen</th>
                <th>Estado</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              @for (l of items(); track l.id) {
                @if (editingId() === l.id) {
                  <tr>
                    <td><input [(ngModel)]="editNombre" /></td>
                    <td><input [(ngModel)]="editEmpresa" /></td>
                    <td><input [(ngModel)]="editEmail" /></td>
                    <td><input [(ngModel)]="editTelefono" /></td>
                    <td><span class="badge badge--neutral">{{ l.origen }}</span></td>
                    <td><span class="badge">{{ l.estado }}</span></td>
                    <td>
                      <button class="btn-primary btn-sm" (click)="saveEdit(l.id)" [disabled]="savingEdit()">Guardar</button>
                      <button class="btn-outline btn-sm" (click)="cancelEdit()">Cancelar</button>
                    </td>
                  </tr>
                } @else {
                  <tr>
                    <td>{{ l.nombre }}</td>
                    <td>{{ l.empresa ?? '---' }}</td>
                    <td>{{ l.email ?? '---' }}</td>
                    <td>{{ l.telefono ?? '---' }}</td>
                    <td><span class="badge badge--neutral">{{ l.origen }}</span></td>
                    <td><span class="badge">{{ l.estado }}</span></td>
                    <td>
                      <button class="btn-outline btn-sm" (click)="startEdit(l)">Editar</button>
                      <button class="btn-outline btn-sm" style="background:#fee;color:#c00;" (click)="remove(l)">Eliminar</button>
                    </td>
                  </tr>
                }
              } @empty {
                <tr><td colspan="7" class="empty-state">Sin leads</td></tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `,
})
export class LeadsListComponent implements OnInit {
  private svc = inject(CrmService);
  private exportSvc = inject(ExportService);

  exportCsv(): void {
    const rows = this.items().map(l => [l.nombre, (l as any).empresa ?? '', (l as any).email ?? '', (l as any).telefono ?? '', l.origen, l.estado]);
    this.exportSvc.exportToCsv('leads', ['Nombre', 'Empresa', 'Email', 'Telefono', 'Origen', 'Estado'], rows);
  }

  readonly items = signal<LeadListDto[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly showCreate = signal(false);
  readonly saving = signal(false);
  readonly createError = signal<string | null>(null);

  estados: EstadoLead[] = ['Nuevo', 'Contactado', 'Calificado', 'Descartado', 'Convertido'];
  origenes: OrigenLead[] = ['Web', 'Recomendacion', 'Evento', 'LlamadaFria', 'Marketing', 'Otro'];

  filterSearch = '';
  filterEstado = '';

  newNombre = '';
  newOrigen: OrigenLead = 'Web';
  newEmpresa = '';
  newEmail = '';
  newTelefono = '';

  readonly editingId = signal<string | null>(null);
  readonly savingEdit = signal(false);
  editNombre = '';
  editEmpresa = '';
  editEmail = '';
  editTelefono = '';
  private editNotas: string | null = null;

  startEdit(l: LeadListDto): void {
    this.editingId.set(l.id);
    this.editNombre = l.nombre;
    this.editEmpresa = l.empresa ?? '';
    this.editEmail = l.email ?? '';
    this.editTelefono = l.telefono ?? '';
    this.editNotas = l.notas;
  }
  cancelEdit(): void { this.editingId.set(null); }
  saveEdit(id: string): void {
    this.savingEdit.set(true);
    this.svc.updateLead(id, {
      nombre: this.editNombre,
      empresa: this.editEmpresa || null,
      email: this.editEmail || null,
      telefono: this.editTelefono || null,
      notas: this.editNotas,
    }).subscribe({
      next: () => { this.savingEdit.set(false); this.editingId.set(null); this.load(); },
      error: (err) => { this.savingEdit.set(false); this.error.set(err?.error?.message ?? 'Error al actualizar'); },
    });
  }
  remove(l: LeadListDto): void {
    if (!confirm(`¿Eliminar lead "${l.nombre}"?`)) return;
    this.svc.deleteLead(l.id).subscribe({
      next: () => this.load(),
      error: (err) => this.error.set(err?.error?.message ?? 'Error al eliminar'),
    });
  }

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.svc.listLeads({
      search: this.filterSearch || undefined,
      estado: (this.filterEstado as EstadoLead) || undefined,
    }).subscribe({
      next: (r) => { this.items.set(r.items); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar leads'); this.loading.set(false); },
    });
  }

  toggleCreate(): void {
    this.showCreate.update(v => !v);
    this.createError.set(null);
  }

  save(): void {
    if (!this.newNombre) {
      this.createError.set('Nombre obligatorio');
      return;
    }
    this.saving.set(true);
    this.createError.set(null);
    this.svc.createLead({
      nombre: this.newNombre,
      origen: this.newOrigen,
      empresa: this.newEmpresa || undefined,
      email: this.newEmail || undefined,
      telefono: this.newTelefono || undefined,
    }).subscribe({
      next: () => {
        this.saving.set(false);
        this.showCreate.set(false);
        this.newNombre = ''; this.newEmpresa = ''; this.newEmail = ''; this.newTelefono = '';
        this.load();
      },
      error: (err) => {
        this.createError.set(err?.error?.message ?? 'Error al crear lead');
        this.saving.set(false);
      },
    });
  }
}
