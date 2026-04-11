import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ProduccionService, OFListDto } from '../../../core/services/produccion.service';
import { NotificationService } from '../../../shared/components/toast/notification.service';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-ordenes-fab-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ReactiveFormsModule, PaginationComponent],
  template: `
    <div class="page">
      <div class="page__header">
        <div>
          <h1 class="page__title">Ordenes de Fabricacion</h1>
          <p class="page__subtitle">Gestion de ordenes de produccion</p>
        </div>
        <button class="btn-primary" (click)="showForm.set(true)">+ Nueva Orden</button>
      </div>

      <div class="page__filters" style="display: flex; gap: 0.75rem; flex-wrap: wrap;">
        <input class="filter-input" placeholder="Buscar por codigo..."
               (input)="onSearch($event)" />
        <select class="filter-select" (change)="onStatusFilter($event)">
          <option value="">Todos los estados</option>
          <option value="Planificada">Planificada</option>
          <option value="EnPreparacion">En Preparacion</option>
          <option value="EnCurso">En Curso</option>
          <option value="Pausada">Pausada</option>
          <option value="Completada">Completada</option>
          <option value="Cancelada">Cancelada</option>
        </select>
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando ordenes...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Codigo</th>
                <th>Cantidad</th>
                <th>Unidad</th>
                <th>Estado</th>
                <th>Fecha Inicio</th>
                <th>Prioridad</th>
              </tr>
            </thead>
            <tbody>
              @for (item of items(); track item.id) {
                <tr class="clickable-row" (click)="goToDetail(item.id)">
                  <td><code>{{ item.codigoOF }}</code></td>
                  <td>{{ item.cantidadPlanificada }}</td>
                  <td>{{ item.unidadMedida }}</td>
                  <td>
                    <span class="badge"
                      [class.badge--info]="item.estadoOF === 'Planificada'"
                      [class.badge--warning]="item.estadoOF === 'EnCurso' || item.estadoOF === 'EnPreparacion'"
                      [class.badge--success]="item.estadoOF === 'Completada'"
                      [class.badge--neutral]="item.estadoOF === 'Cancelada' || item.estadoOF === 'Pausada'">
                      {{ item.estadoOF }}
                    </span>
                  </td>
                  <td>{{ item.fechaInicio ? (item.fechaInicio | date:'dd/MM/yyyy') : '---' }}</td>
                  <td>{{ item.prioridad }}</td>
                </tr>
              } @empty {
                <tr><td colspan="6" class="empty-state">No se encontraron resultados</td></tr>
              }
            </tbody>
          </table>
        </div>

        @if (totalPages() > 1) {
          <app-pagination [currentPage]="currentPage()" [totalPages]="totalPages()" (pageChange)="onPageChange($event)" />
        }

        <div class="page__footer">{{ items().length }} de {{ totalCount() }} registros</div>
      }
    </div>

    @if (showForm()) {
      <div class="form-overlay" (click)="closeForm()">
        <div class="form-dialog" (click)="$event.stopPropagation()">
          <h2 class="form-dialog__title">Nueva Orden de Fabricacion</h2>
          <form [formGroup]="form" (ngSubmit)="onSubmit()">
            <div class="form-group">
              <label class="form-label">Producto ID (GUID) *</label>
              <input class="form-input" formControlName="productoId" placeholder="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" />
            </div>
            <div class="form-group">
              <label class="form-label">Lista Materiales ID (GUID)</label>
              <input class="form-input" formControlName="listaMaterialesId" />
            </div>
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Cantidad Planificada *</label>
                <input class="form-input" type="number" formControlName="cantidadPlanificada" />
              </div>
              <div class="form-group">
                <label class="form-label">Unidad Medida *</label>
                <select class="form-select" formControlName="unidadMedida">
                  <option value="">Seleccionar...</option>
                  <option value="kg">kg</option>
                  <option value="ud">ud</option>
                  <option value="m">m</option>
                  <option value="l">l</option>
                </select>
              </div>
            </div>
            <div class="form-group">
              <label class="form-label">Prioridad (1-5) *</label>
              <input class="form-input" type="number" min="1" max="5" formControlName="prioridad" />
            </div>
            <div class="form-group">
              <label class="form-label">Observaciones</label>
              <textarea class="form-textarea" formControlName="observaciones"></textarea>
            </div>
            <div class="form-actions">
              <button type="button" class="btn-outline" (click)="closeForm()">Cancelar</button>
              <button type="submit" class="btn-primary" [disabled]="form.invalid || submitting()">
                {{ submitting() ? 'Creando...' : 'Crear Orden' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    }
  `,
})
export class OrdenesListComponent implements OnInit {
  private svc = inject(ProduccionService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private notify = inject(NotificationService);

  readonly items = signal<OFListDto[]>([]);
  readonly totalCount = signal(0);
  readonly currentPage = signal(1);
  readonly totalPages = signal(1);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly showForm = signal(false);
  readonly submitting = signal(false);
  private currentSearch = '';
  private currentStatus = '';
  private searchTimeout: any;

  readonly form = this.fb.nonNullable.group({
    productoId: ['', Validators.required],
    listaMaterialesId: [''],
    cantidadPlanificada: [1, [Validators.required, Validators.min(1)]],
    unidadMedida: ['', Validators.required],
    prioridad: [3, [Validators.required, Validators.min(1), Validators.max(5)]],
    observaciones: [''],
  });

  ngOnInit(): void { this.load(); }

  onSearch(event: Event): void {
    clearTimeout(this.searchTimeout);
    this.currentSearch = (event.target as HTMLInputElement).value;
    this.searchTimeout = setTimeout(() => { this.currentPage.set(1); this.load(); }, 300);
  }

  onStatusFilter(event: Event): void {
    this.currentStatus = (event.target as HTMLSelectElement).value;
    this.currentPage.set(1);
    this.load();
  }

  onPageChange(page: number): void { this.currentPage.set(page); this.load(); }
  goToDetail(id: string): void { this.router.navigate(['/app/produccion/ordenes', id]); }

  closeForm(): void { this.showForm.set(false); this.form.reset({ prioridad: 3, cantidadPlanificada: 1 }); }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.submitting.set(true);
    const v = this.form.getRawValue();
    this.svc.createOF({
      productoId: v.productoId,
      listaMaterialesId: v.listaMaterialesId || undefined,
      cantidadPlanificada: v.cantidadPlanificada,
      unidadMedida: v.unidadMedida,
      prioridad: v.prioridad,
      observaciones: v.observaciones || undefined,
    }).subscribe({
      next: () => { this.notify.success('Orden creada correctamente'); this.closeForm(); this.submitting.set(false); this.load(); },
      error: (err) => { this.notify.error(err?.error?.message ?? 'Error al crear orden'); this.submitting.set(false); },
    });
  }

  private load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.svc.listOFs(this.currentSearch || undefined, this.currentStatus || undefined, this.currentPage()).subscribe({
      next: (res) => { this.items.set(res.items); this.totalCount.set(res.totalCount); this.totalPages.set(res.totalPages); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar ordenes'); this.loading.set(false); },
    });
  }
}
