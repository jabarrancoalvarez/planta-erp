import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ComprasService, OCListDto } from '../../../core/services/compras.service';
import { NotificationService } from '../../../shared/components/toast/notification.service';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-oc-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ReactiveFormsModule, PaginationComponent],
  template: `
    <div class="page">
      <div class="page__header">
        <div>
          <h1 class="page__title">Ordenes de Compra</h1>
          <p class="page__subtitle">Gestion de pedidos a proveedores</p>
        </div>
        <button class="btn-primary" (click)="showForm.set(true)">+ Nueva Orden</button>
      </div>

      <div class="page__filters">
        <input class="filter-input" placeholder="Buscar por codigo o proveedor..."
               (input)="onSearch($event)" />
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando ordenes de compra...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Codigo</th>
                <th>Proveedor</th>
                <th>Fecha Emision</th>
                <th>Total</th>
                <th>Estado</th>
              </tr>
            </thead>
            <tbody>
              @for (item of items(); track item.id) {
                <tr class="clickable-row" (click)="goToDetail(item.id)">
                  <td><code>{{ item.codigo }}</code></td>
                  <td>{{ item.proveedorRazonSocial }}</td>
                  <td>{{ item.fechaEmision | date:'dd/MM/yyyy' }}</td>
                  <td>{{ item.total | number:'1.2-2' }} &euro;</td>
                  <td>
                    <span class="badge"
                      [class.badge--warning]="item.estadoOC === 'Pendiente' || item.estadoOC === 'Borrador'"
                      [class.badge--info]="item.estadoOC === 'Enviada'"
                      [class.badge--success]="item.estadoOC === 'Recibida'"
                      [class.badge--neutral]="item.estadoOC === 'Cancelada'">
                      {{ item.estadoOC }}
                    </span>
                  </td>
                </tr>
              } @empty {
                <tr><td colspan="5" class="empty-state">No se encontraron resultados</td></tr>
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
          <h2 class="form-dialog__title">Nueva Orden de Compra</h2>
          <form [formGroup]="form" (ngSubmit)="onSubmit()">
            <div class="form-group">
              <label class="form-label">Proveedor ID (GUID) *</label>
              <input class="form-input" formControlName="proveedorId" placeholder="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" />
            </div>
            <div class="form-group">
              <label class="form-label">Fecha Entrega Estimada</label>
              <input class="form-input" type="date" formControlName="fechaEntregaEstimada" />
            </div>
            <div class="form-group">
              <label class="form-label">Observaciones</label>
              <textarea class="form-textarea" formControlName="observaciones"></textarea>
            </div>
            <div class="form-actions">
              <button type="button" class="btn-outline" (click)="closeForm()">Cancelar</button>
              <button type="submit" class="btn-primary" [disabled]="form.invalid || submitting()">
                {{ submitting() ? 'Creando...' : 'Crear OC' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    }
  `,
})
export class OcListComponent implements OnInit {
  private svc = inject(ComprasService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private notify = inject(NotificationService);

  readonly items = signal<OCListDto[]>([]);
  readonly totalCount = signal(0);
  readonly currentPage = signal(1);
  readonly totalPages = signal(1);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly showForm = signal(false);
  readonly submitting = signal(false);
  private searchTimeout: any;
  private currentSearch = '';

  readonly form = this.fb.nonNullable.group({
    proveedorId: ['', Validators.required],
    fechaEntregaEstimada: [''],
    observaciones: [''],
  });

  ngOnInit(): void { this.load(); }

  onSearch(event: Event): void {
    clearTimeout(this.searchTimeout);
    this.currentSearch = (event.target as HTMLInputElement).value;
    this.searchTimeout = setTimeout(() => { this.currentPage.set(1); this.load(); }, 300);
  }

  onPageChange(page: number): void { this.currentPage.set(page); this.load(); }
  goToDetail(id: string): void { this.router.navigate(['/app/compras/ordenes', id]); }
  closeForm(): void { this.showForm.set(false); this.form.reset(); }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.submitting.set(true);
    const v = this.form.getRawValue();
    this.svc.createOC({
      proveedorId: v.proveedorId,
      fechaEntregaEstimada: v.fechaEntregaEstimada || undefined,
      observaciones: v.observaciones || undefined,
    }).subscribe({
      next: () => { this.notify.success('Orden de compra creada'); this.closeForm(); this.submitting.set(false); this.load(); },
      error: (err) => { this.notify.error(err?.error?.message ?? 'Error al crear OC'); this.submitting.set(false); },
    });
  }

  private load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.svc.listOCs(this.currentSearch || undefined, undefined, this.currentPage()).subscribe({
      next: (res) => { this.items.set(res.items); this.totalCount.set(res.totalCount); this.totalPages.set(res.totalPages); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar ordenes de compra'); this.loading.set(false); },
    });
  }
}
