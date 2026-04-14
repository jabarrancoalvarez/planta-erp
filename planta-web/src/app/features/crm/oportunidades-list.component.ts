import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule, DecimalPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CrmService, OportunidadListDto, FaseOportunidad } from '../../core/services/crm.service';

@Component({
  selector: 'app-oportunidades-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule, DecimalPipe, DatePipe],
  template: `
    <div class="list-page">
      <div class="list-page__header">
        <h1 class="list-page__title">Oportunidades CRM</h1>
        <button class="btn-primary" (click)="toggleCreate()">{{ showCreate() ? 'Cancelar' : '+ Nueva oportunidad' }}</button>
      </div>

      @if (showCreate()) {
        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Crear oportunidad</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <label class="detail-page__field-label">Titulo *</label>
              <input [(ngModel)]="newTitulo" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Importe estimado *</label>
              <input type="number" [(ngModel)]="newImporte" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Descripcion</label>
              <input [(ngModel)]="newDescripcion" />
            </div>
          </div>
          @if (createError()) { <div class="error-state">{{ createError() }}</div> }
          <div style="display:flex; gap:0.5rem; margin-top:1rem;">
            <button class="btn-primary" (click)="save()" [disabled]="saving()">{{ saving() ? 'Creando...' : 'Crear' }}</button>
          </div>
        </div>
      }

      <div class="filters-bar">
        <select [(ngModel)]="filterFase" (ngModelChange)="load()">
          <option value="">Todas las fases</option>
          @for (f of fases; track f) { <option [value]="f">{{ f }}</option> }
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
                <th>Titulo</th>
                <th>Fase</th>
                <th style="text-align:right">Importe</th>
                <th style="text-align:right">Prob %</th>
                <th style="text-align:right">Ponderado</th>
                <th>Cierre est.</th>
                <th>Avanzar</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              @for (o of items(); track o.id) {
                @if (editingId() === o.id) {
                  <tr>
                    <td><input [(ngModel)]="editTitulo" /></td>
                    <td><span class="badge">{{ o.fase }}</span></td>
                    <td><input type="number" [(ngModel)]="editImporte" /></td>
                    <td style="text-align:right">{{ o.probabilidadPct }}</td>
                    <td style="text-align:right">---</td>
                    <td><input type="date" [(ngModel)]="editFecha" /></td>
                    <td>---</td>
                    <td>
                      <button class="btn-primary btn-sm" (click)="saveEdit(o.id)" [disabled]="savingEdit()">Guardar</button>
                      <button class="btn-outline btn-sm" (click)="cancelEdit()">Cancelar</button>
                    </td>
                  </tr>
                } @else {
                  <tr>
                    <td>{{ o.titulo }}</td>
                    <td><span class="badge">{{ o.fase }}</span></td>
                    <td style="text-align:right">{{ o.importeEstimado | number:'1.2-2' }} &euro;</td>
                    <td style="text-align:right">{{ o.probabilidadPct }}</td>
                    <td style="text-align:right">{{ o.valorPonderado | number:'1.2-2' }} &euro;</td>
                    <td>{{ o.fechaCierreEstimada ? (o.fechaCierreEstimada | date:'dd/MM/yyyy') : '---' }}</td>
                    <td>
                      <select (change)="avanzar(o, $any($event.target).value)">
                        <option value="">--</option>
                        @for (f of fases; track f) { <option [value]="f">{{ f }}</option> }
                      </select>
                    </td>
                    <td>
                      <button class="btn-outline btn-sm" (click)="startEdit(o)">Editar</button>
                      <button class="btn-outline btn-sm" style="background:#fee;color:#c00;" (click)="remove(o)">Eliminar</button>
                    </td>
                  </tr>
                }
              } @empty {
                <tr><td colspan="8" class="empty-state">Sin oportunidades</td></tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `,
})
export class OportunidadesListComponent implements OnInit {
  private svc = inject(CrmService);

  readonly items = signal<OportunidadListDto[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly showCreate = signal(false);
  readonly saving = signal(false);
  readonly createError = signal<string | null>(null);

  fases: FaseOportunidad[] = ['Prospecto', 'Contactado', 'PropuestaEnviada', 'Negociacion', 'Ganada', 'Perdida'];
  filterFase = '';

  newTitulo = '';
  newImporte = 0;
  newDescripcion = '';

  readonly editingId = signal<string | null>(null);
  readonly savingEdit = signal(false);
  editTitulo = '';
  editImporte = 0;
  editFecha = '';
  private editDescripcion: string | null = null;

  startEdit(o: OportunidadListDto): void {
    this.editingId.set(o.id);
    this.editTitulo = o.titulo;
    this.editImporte = o.importeEstimado;
    this.editFecha = o.fechaCierreEstimada ? new Date(o.fechaCierreEstimada).toISOString().slice(0, 10) : '';
    this.editDescripcion = o.descripcion;
  }
  cancelEdit(): void { this.editingId.set(null); }
  saveEdit(id: string): void {
    this.savingEdit.set(true);
    this.svc.updateOportunidad(id, {
      titulo: this.editTitulo,
      importeEstimado: Number(this.editImporte),
      fechaCierreEstimada: this.editFecha ? new Date(this.editFecha).toISOString() : null,
      descripcion: this.editDescripcion,
    }).subscribe({
      next: () => { this.savingEdit.set(false); this.editingId.set(null); this.load(); },
      error: (err) => { this.savingEdit.set(false); this.error.set(err?.error?.message ?? 'Error al actualizar'); },
    });
  }
  remove(o: OportunidadListDto): void {
    if (!confirm(`¿Eliminar oportunidad "${o.titulo}"?`)) return;
    this.svc.deleteOportunidad(o.id).subscribe({
      next: () => this.load(),
      error: (err) => this.error.set(err?.error?.message ?? 'Error al eliminar'),
    });
  }

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.svc.listOportunidades({ fase: (this.filterFase as FaseOportunidad) || undefined }).subscribe({
      next: (r) => { this.items.set(r.items); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar'); this.loading.set(false); },
    });
  }

  toggleCreate(): void {
    this.showCreate.update(v => !v);
    this.createError.set(null);
  }

  save(): void {
    if (!this.newTitulo) {
      this.createError.set('Titulo obligatorio');
      return;
    }
    this.saving.set(true);
    this.svc.createOportunidad({
      titulo: this.newTitulo,
      importeEstimado: this.newImporte,
      descripcion: this.newDescripcion || undefined,
    }).subscribe({
      next: () => {
        this.saving.set(false);
        this.showCreate.set(false);
        this.newTitulo = ''; this.newImporte = 0; this.newDescripcion = '';
        this.load();
      },
      error: (err) => {
        this.createError.set(err?.error?.message ?? 'Error al crear');
        this.saving.set(false);
      },
    });
  }

  avanzar(o: OportunidadListDto, fase: string): void {
    if (!fase) return;
    this.svc.avanzarFase(o.id, fase as FaseOportunidad).subscribe({
      next: () => this.load(),
      error: (e) => this.error.set(e?.error?.message ?? 'Error avanzar'),
    });
  }
}
