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
              </tr>
            </thead>
            <tbody>
              @for (i of items(); track i.id) {
                <tr>
                  <td>{{ i.fecha | date:'dd/MM/yyyy' }}</td>
                  <td><span class="badge">{{ i.tipo }}</span></td>
                  <td>{{ i.origen }}</td>
                  <td>{{ i.concepto ?? '---' }}</td>
                  <td style="text-align:right">{{ i.cantidad | number:'1.2-2' }}</td>
                  <td style="text-align:right">{{ i.precioUnitario | number:'1.2-2' }} &euro;</td>
                  <td style="text-align:right"><strong>{{ i.importe | number:'1.2-2' }} &euro;</strong></td>
                </tr>
              } @empty {
                <tr><td colspan="7" class="empty-state">Sin imputaciones</td></tr>
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
