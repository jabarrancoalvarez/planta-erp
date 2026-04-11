import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { VentasService, PedidoListDto } from '../../../core/services/ventas.service';
import { NotificationService } from '../../../shared/components/toast/notification.service';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-pedidos-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ReactiveFormsModule, PaginationComponent],
  template: `
    <div class="page">
      <div class="page__header">
        <div>
          <h1 class="page__title">Pedidos de Venta</h1>
          <p class="page__subtitle">Gestion de pedidos de clientes</p>
        </div>
        <button class="btn-primary" (click)="showForm.set(true)">+ Nuevo Pedido</button>
      </div>

      <div class="page__filters">
        <input class="filter-input" placeholder="Buscar por codigo o cliente..."
               (input)="onSearch($event)" />
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando pedidos...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Codigo</th>
                <th>Cliente</th>
                <th>Fecha Emision</th>
                <th>Total</th>
                <th>Estado</th>
              </tr>
            </thead>
            <tbody>
              @for (item of items(); track item.id) {
                <tr class="clickable-row" (click)="goToDetail(item.id)">
                  <td><code>{{ item.codigo }}</code></td>
                  <td>{{ item.clienteRazonSocial }}</td>
                  <td>{{ item.fechaEmision | date:'dd/MM/yyyy' }}</td>
                  <td>{{ item.total | number:'1.2-2' }} &euro;</td>
                  <td>
                    <span class="badge"
                      [class.badge--info]="item.estadoPedido === 'Confirmado'"
                      [class.badge--warning]="item.estadoPedido === 'EnPreparacion' || item.estadoPedido === 'Borrador'"
                      [class.badge--success]="item.estadoPedido === 'Expedido' || item.estadoPedido === 'Entregado'"
                      [class.badge--neutral]="item.estadoPedido === 'Cancelado'">
                      {{ item.estadoPedido }}
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
          <h2 class="form-dialog__title">Nuevo Pedido de Venta</h2>
          <form [formGroup]="form" (ngSubmit)="onSubmit()">
            <div class="form-group">
              <label class="form-label">Cliente ID (GUID) *</label>
              <input class="form-input" formControlName="clienteId" placeholder="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" />
            </div>
            <div class="form-group">
              <label class="form-label">Fecha Entrega Estimada</label>
              <input class="form-input" type="date" formControlName="fechaEntregaEstimada" />
            </div>
            <div class="form-group">
              <label class="form-label">Direccion de Entrega</label>
              <input class="form-input" formControlName="direccionEntrega" />
            </div>
            <div class="form-group">
              <label class="form-label">Observaciones</label>
              <textarea class="form-textarea" formControlName="observaciones"></textarea>
            </div>
            <div class="form-actions">
              <button type="button" class="btn-outline" (click)="closeForm()">Cancelar</button>
              <button type="submit" class="btn-primary" [disabled]="form.invalid || submitting()">
                {{ submitting() ? 'Creando...' : 'Crear Pedido' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    }
  `,
})
export class PedidosListComponent implements OnInit {
  private svc = inject(VentasService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private notify = inject(NotificationService);

  readonly items = signal<PedidoListDto[]>([]);
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
    clienteId: ['', Validators.required],
    fechaEntregaEstimada: [''],
    direccionEntrega: [''],
    observaciones: [''],
  });

  ngOnInit(): void { this.load(); }

  onSearch(event: Event): void {
    clearTimeout(this.searchTimeout);
    this.currentSearch = (event.target as HTMLInputElement).value;
    this.searchTimeout = setTimeout(() => { this.currentPage.set(1); this.load(); }, 300);
  }

  onPageChange(page: number): void { this.currentPage.set(page); this.load(); }
  goToDetail(id: string): void { this.router.navigate(['/app/ventas/pedidos', id]); }
  closeForm(): void { this.showForm.set(false); this.form.reset(); }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.submitting.set(true);
    const v = this.form.getRawValue();
    this.svc.createPedido({
      clienteId: v.clienteId,
      fechaEntregaEstimada: v.fechaEntregaEstimada || undefined,
      direccionEntrega: v.direccionEntrega || undefined,
      observaciones: v.observaciones || undefined,
    }).subscribe({
      next: () => { this.notify.success('Pedido creado correctamente'); this.closeForm(); this.submitting.set(false); this.load(); },
      error: (err) => { this.notify.error(err?.error?.message ?? 'Error al crear pedido'); this.submitting.set(false); },
    });
  }

  private load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.svc.listPedidos(this.currentSearch || undefined, undefined, this.currentPage()).subscribe({
      next: (res) => { this.items.set(res.items); this.totalCount.set(res.totalCount); this.totalPages.set(res.totalPages); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar pedidos'); this.loading.set(false); },
    });
  }
}
