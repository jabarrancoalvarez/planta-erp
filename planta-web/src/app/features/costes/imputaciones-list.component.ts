import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CostesService, ImputacionCosteDto, TipoCoste, OrigenImputacion } from '../../core/services/costes.service';

@Component({
  selector: 'app-imputaciones-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule, DatePipe, DecimalPipe],
  template: `
    <div class="list-page">
      <div class="list-page__header">
        <h1 class="list-page__title">Imputaciones de Coste</h1>
        <button class="btn-primary" (click)="toggleCreate()">{{ showCreate() ? 'Cancelar' : '+ Nueva imputacion' }}</button>
      </div>

      @if (showCreate()) {
        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Crear imputacion</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <label class="detail-page__field-label">Tipo *</label>
              <select [(ngModel)]="newTipo">
                @for (t of tipos; track t) { <option [value]="t">{{ t }}</option> }
              </select>
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Origen *</label>
              <select [(ngModel)]="newOrigen">
                @for (o of origenes; track o) { <option [value]="o">{{ o }}</option> }
              </select>
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Cantidad</label>
              <input type="number" [(ngModel)]="newCantidad" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Precio unitario</label>
              <input type="number" [(ngModel)]="newPrecio" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Concepto</label>
              <input [(ngModel)]="newConcepto" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">OF Id (opt)</label>
              <input [(ngModel)]="newOfId" />
            </div>
          </div>
          @if (createError()) { <div class="error-state">{{ createError() }}</div> }
          <div style="display:flex; gap:0.5rem; margin-top:1rem;">
            <button class="btn-primary" (click)="save()" [disabled]="saving()">{{ saving() ? 'Creando...' : 'Crear' }}</button>
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
                <th>Fecha</th>
                <th>Tipo</th>
                <th>Origen</th>
                <th>Concepto</th>
                <th style="text-align:right">Cantidad</th>
                <th style="text-align:right">Precio</th>
                <th style="text-align:right">Importe</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              @for (i of items(); track i.id) {
                @if (editingId() === i.id) {
                  <tr>
                    <td><input type="date" [(ngModel)]="editFecha" /></td>
                    <td>{{ i.tipo }}</td>
                    <td>{{ i.origen }}</td>
                    <td><input [(ngModel)]="editConcepto" /></td>
                    <td><input type="number" step="0.01" [(ngModel)]="editCantidad" /></td>
                    <td><input type="number" step="0.01" [(ngModel)]="editPrecio" /></td>
                    <td style="text-align:right">---</td>
                    <td>
                      <button class="btn-primary btn-sm" (click)="saveEdit(i.id)" [disabled]="savingEdit()">Guardar</button>
                      <button class="btn-outline btn-sm" (click)="cancelEdit()">Cancelar</button>
                    </td>
                  </tr>
                } @else {
                  <tr>
                    <td>{{ i.fecha | date:'dd/MM/yyyy' }}</td>
                    <td><span class="badge">{{ i.tipo }}</span></td>
                    <td>{{ i.origen }}</td>
                    <td>{{ i.concepto ?? '---' }}</td>
                    <td style="text-align:right">{{ i.cantidad | number:'1.2-2' }}</td>
                    <td style="text-align:right">{{ i.precioUnitario | number:'1.2-2' }} &euro;</td>
                    <td style="text-align:right"><strong>{{ i.importe | number:'1.2-2' }} &euro;</strong></td>
                    <td>
                      <button class="btn-outline btn-sm" (click)="startEdit(i)">Editar</button>
                      <button class="btn-outline btn-sm" style="background:#fee;color:#c00;" (click)="remove(i)">Eliminar</button>
                    </td>
                  </tr>
                }
              } @empty {
                <tr><td colspan="8" class="empty-state">Sin imputaciones</td></tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `,
})
export class ImputacionesListComponent implements OnInit {
  private svc = inject(CostesService);

  readonly items = signal<ImputacionCosteDto[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly showCreate = signal(false);
  readonly saving = signal(false);
  readonly createError = signal<string | null>(null);

  tipos: TipoCoste[] = ['Material', 'ManoObra', 'Maquina', 'Subcontratacion', 'Indirecto', 'Otro'];
  origenes: OrigenImputacion[] = ['Manual', 'FichajeOperario', 'ConsumoStock', 'ParteProduccion', 'OrdenTrabajo'];

  newTipo: TipoCoste = 'Material';
  newOrigen: OrigenImputacion = 'Manual';
  newCantidad = 1;
  newPrecio = 0;
  newConcepto = '';
  newOfId = '';

  readonly editingId = signal<string | null>(null);
  readonly savingEdit = signal(false);
  editCantidad = 0;
  editPrecio = 0;
  editConcepto = '';
  editFecha = '';

  startEdit(i: ImputacionCosteDto): void {
    this.editingId.set(i.id);
    this.editCantidad = i.cantidad;
    this.editPrecio = i.precioUnitario;
    this.editConcepto = i.concepto ?? '';
    this.editFecha = new Date(i.fecha).toISOString().slice(0, 10);
  }
  cancelEdit(): void { this.editingId.set(null); }
  saveEdit(id: string): void {
    this.savingEdit.set(true);
    this.svc.updateImputacion(id, {
      cantidad: Number(this.editCantidad),
      precioUnitario: Number(this.editPrecio),
      concepto: this.editConcepto || null,
      fecha: new Date(this.editFecha).toISOString(),
    }).subscribe({
      next: () => { this.savingEdit.set(false); this.editingId.set(null); this.load(); },
      error: (err) => { this.savingEdit.set(false); this.error.set(err?.error?.message ?? 'Error al actualizar'); },
    });
  }
  remove(i: ImputacionCosteDto): void {
    if (!confirm(`¿Eliminar imputación de ${i.importe.toFixed(2)} €?`)) return;
    this.svc.deleteImputacion(i.id).subscribe({
      next: () => this.load(),
      error: (err) => this.error.set(err?.error?.message ?? 'Error al eliminar'),
    });
  }

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.svc.listImputaciones().subscribe({
      next: (r) => { this.items.set(r.items); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar'); this.loading.set(false); },
    });
  }

  toggleCreate(): void {
    this.showCreate.update(v => !v);
    this.createError.set(null);
  }

  save(): void {
    this.saving.set(true);
    this.svc.createImputacion({
      tipo: this.newTipo,
      origen: this.newOrigen,
      cantidad: this.newCantidad,
      precioUnitario: this.newPrecio,
      concepto: this.newConcepto || undefined,
      ordenFabricacionId: this.newOfId || undefined,
    }).subscribe({
      next: () => {
        this.saving.set(false);
        this.showCreate.set(false);
        this.newConcepto = ''; this.newOfId = '';
        this.load();
      },
      error: (err) => {
        this.createError.set(err?.error?.message ?? 'Error al crear');
        this.saving.set(false);
      },
    });
  }
}
