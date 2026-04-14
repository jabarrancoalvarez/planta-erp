import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OeeService, RegistroOEEDto } from '../../core/services/oee.service';
import { ActivosService, ActivoListDto } from '../../core/services/activos.service';

@Component({
  selector: 'app-oee-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule, DatePipe, DecimalPipe],
  template: `
    <div class="list-page">
      <div class="list-page__header">
        <h1 class="list-page__title">Registros OEE</h1>
        <button class="btn-primary" (click)="toggleCreate()">{{ showCreate() ? 'Cancelar' : '+ Nuevo registro' }}</button>
      </div>

      @if (showCreate()) {
        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Registrar OEE</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <label class="detail-page__field-label">Activo *</label>
              <select [(ngModel)]="newActivoId">
                <option value="">--</option>
                @for (a of activos(); track a.id) {
                  <option [value]="a.id">{{ a.nombre }}</option>
                }
              </select>
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Fecha *</label>
              <input type="date" [(ngModel)]="newFecha" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Min. planificados</label>
              <input type="number" [(ngModel)]="newMinPlan" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Min. funcionamiento</label>
              <input type="number" [(ngModel)]="newMinFunc" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Piezas totales</label>
              <input type="number" [(ngModel)]="newPzTotales" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Piezas buenas</label>
              <input type="number" [(ngModel)]="newPzBuenas" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">T. ciclo teorico (s)</label>
              <input type="number" [(ngModel)]="newCiclo" />
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
                <th>Fecha</th>
                <th>Min. Plan</th>
                <th>Min. Func</th>
                <th>Pz. Total</th>
                <th>Pz. Buenas</th>
                <th style="text-align:right">Disp %</th>
                <th style="text-align:right">Rend %</th>
                <th style="text-align:right">Calidad %</th>
                <th style="text-align:right">OEE %</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              @for (r of items(); track r.id) {
                <tr>
                  <td>{{ r.fecha | date:'dd/MM/yyyy' }}</td>
                  <td>{{ r.minutosPlanificados }}</td>
                  <td>{{ r.minutosFuncionamiento }}</td>
                  <td>{{ r.piezasTotales }}</td>
                  <td>{{ r.piezasBuenas }}</td>
                  <td style="text-align:right">{{ r.disponibilidad | number:'1.1-1' }}</td>
                  <td style="text-align:right">{{ r.rendimiento | number:'1.1-1' }}</td>
                  <td style="text-align:right">{{ r.calidad | number:'1.1-1' }}</td>
                  <td style="text-align:right"><strong>{{ r.oee | number:'1.1-1' }}</strong></td>
                  <td>
                    <button class="btn-outline btn-sm" style="background:#fee;color:#c00;" (click)="remove(r)">Eliminar</button>
                  </td>
                </tr>
              } @empty {
                <tr><td colspan="10" class="empty-state">Sin registros</td></tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `,
})
export class OeeListComponent implements OnInit {
  private svc = inject(OeeService);
  private activosSvc = inject(ActivosService);

  readonly items = signal<RegistroOEEDto[]>([]);
  readonly activos = signal<ActivoListDto[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly showCreate = signal(false);
  readonly saving = signal(false);
  readonly createError = signal<string | null>(null);

  newActivoId = '';
  newFecha = new Date().toISOString().substring(0, 10);
  newMinPlan = 480;
  newMinFunc = 400;
  newPzTotales = 100;
  newPzBuenas = 95;
  newCiclo = 30;

  ngOnInit(): void {
    this.load();
    this.activosSvc.listActivos({ pageSize: 100 }).subscribe({
      next: (r) => this.activos.set(r.items),
      error: () => {},
    });
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.svc.listRegistros().subscribe({
      next: (r) => { this.items.set(r.items); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar'); this.loading.set(false); },
    });
  }

  toggleCreate(): void {
    this.showCreate.update(v => !v);
    this.createError.set(null);
  }

  save(): void {
    if (!this.newActivoId) {
      this.createError.set('Activo obligatorio');
      return;
    }
    this.saving.set(true);
    this.svc.registrar({
      activoId: this.newActivoId,
      fecha: this.newFecha,
      minutosPlanificados: this.newMinPlan,
      minutosFuncionamiento: this.newMinFunc,
      piezasTotales: this.newPzTotales,
      piezasBuenas: this.newPzBuenas,
      tiempoCicloTeoricoSeg: this.newCiclo,
    }).subscribe({
      next: () => {
        this.saving.set(false);
        this.showCreate.set(false);
        this.load();
      },
      error: (err) => {
        this.createError.set(err?.error?.message ?? 'Error al registrar');
        this.saving.set(false);
      },
    });
  }

  remove(r: RegistroOEEDto): void {
    if (!confirm(`¿Eliminar registro OEE del ${new Date(r.fecha).toLocaleDateString()}?`)) return;
    this.svc.deleteRegistro(r.id).subscribe({
      next: () => this.load(),
      error: (err) => this.error.set(err?.error?.message ?? 'Error al eliminar'),
    });
  }
}
