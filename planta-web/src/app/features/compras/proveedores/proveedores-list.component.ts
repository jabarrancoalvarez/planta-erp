import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ComprasService, ProveedorListDto } from '../../../core/services/compras.service';
import { ExportService } from '../../../core/services/export.service';
import { NotificationService } from '../../../shared/components/toast/notification.service';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-proveedores-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ReactiveFormsModule, PaginationComponent],
  template: `
    <div class="page">
      <div class="page__header">
        <div>
          <h1 class="page__title">Proveedores</h1>
          <p class="page__subtitle">Gestion de proveedores y condiciones de compra</p>
        </div>
        <div style="display:flex; gap:0.5rem;">
          <button class="btn-outline" (click)="exportCsv()">Exportar CSV</button>
          <button class="btn-primary" (click)="showForm.set(true)">+ Nuevo Proveedor</button>
        </div>
      </div>

      <div class="page__filters">
        <input class="filter-input" placeholder="Buscar por nombre o NIF..."
               (input)="onSearch($event)" />
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando proveedores...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>NIF</th>
                <th>Razon Social</th>
                <th>Ciudad</th>
                <th>Email</th>
                <th>Estado</th>
              </tr>
            </thead>
            <tbody>
              @for (item of items(); track item.id) {
                <tr class="clickable-row" (click)="goToDetail(item.id)">
                  <td><code>{{ item.nif }}</code></td>
                  <td>{{ item.razonSocial }}</td>
                  <td>{{ item.ciudad ?? '---' }}</td>
                  <td>{{ item.email }}</td>
                  <td>
                    <span class="active-dot" [class.active-dot--active]="item.activo" [class.active-dot--inactive]="!item.activo"></span>
                    {{ item.activo ? 'Activo' : 'Inactivo' }}
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
          <h2 class="form-dialog__title">Nuevo Proveedor</h2>
          <form [formGroup]="form" (ngSubmit)="onSubmit()">
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Razon Social *</label>
                <input class="form-input" formControlName="razonSocial" />
              </div>
              <div class="form-group">
                <label class="form-label">NIF *</label>
                <input class="form-input" formControlName="nif" />
              </div>
            </div>
            <div class="form-group">
              <label class="form-label">Email *</label>
              <input class="form-input" type="email" formControlName="email" />
            </div>
            <div class="form-group">
              <label class="form-label">Direccion</label>
              <input class="form-input" formControlName="direccion" />
            </div>
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Ciudad</label>
                <input class="form-input" formControlName="ciudad" />
              </div>
              <div class="form-group">
                <label class="form-label">Codigo Postal</label>
                <input class="form-input" formControlName="codigoPostal" />
              </div>
            </div>
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Pais</label>
                <input class="form-input" formControlName="pais" />
              </div>
              <div class="form-group">
                <label class="form-label">Telefono</label>
                <input class="form-input" formControlName="telefono" />
              </div>
            </div>
            <div class="form-group">
              <label class="form-label">Web</label>
              <input class="form-input" formControlName="web" />
            </div>
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Dias Vencimiento</label>
                <input class="form-input" type="number" formControlName="diasVencimiento" />
              </div>
              <div class="form-group">
                <label class="form-label">Descuento Pronto Pago (%)</label>
                <input class="form-input" type="number" step="0.01" formControlName="descuentoProntoPago" />
              </div>
            </div>
            <div class="form-group">
              <label class="form-label">Metodo Pago</label>
              <select class="form-select" formControlName="metodoPago">
                <option value="">Seleccionar...</option>
                <option value="Transferencia">Transferencia</option>
                <option value="Domiciliacion">Domiciliacion</option>
                <option value="Cheque">Cheque</option>
                <option value="Efectivo">Efectivo</option>
              </select>
            </div>
            <div class="form-actions">
              <button type="button" class="btn-outline" (click)="closeForm()">Cancelar</button>
              <button type="submit" class="btn-primary" [disabled]="form.invalid || submitting()">
                {{ submitting() ? 'Creando...' : 'Crear Proveedor' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    }
  `,
})
export class ProveedoresListComponent implements OnInit {
  private svc = inject(ComprasService);
  private router = inject(Router);
  private exportSvc = inject(ExportService);

  exportCsv(): void {
    const rows = this.items().map(p => [p.nif, p.razonSocial, p.ciudad ?? '', p.email, p.activo ? 'Activo' : 'Inactivo']);
    this.exportSvc.exportToCsv('proveedores', ['NIF', 'Razon Social', 'Ciudad', 'Email', 'Estado'], rows);
  }
  private fb = inject(FormBuilder);
  private notify = inject(NotificationService);

  readonly items = signal<ProveedorListDto[]>([]);
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
    razonSocial: ['', Validators.required],
    nif: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    direccion: [''],
    ciudad: [''],
    codigoPostal: [''],
    pais: [''],
    telefono: [''],
    web: [''],
    diasVencimiento: [30],
    descuentoProntoPago: [0],
    metodoPago: [''],
  });

  ngOnInit(): void { this.load(); }

  onSearch(event: Event): void {
    clearTimeout(this.searchTimeout);
    this.currentSearch = (event.target as HTMLInputElement).value;
    this.searchTimeout = setTimeout(() => { this.currentPage.set(1); this.load(); }, 300);
  }

  onPageChange(page: number): void { this.currentPage.set(page); this.load(); }
  goToDetail(id: string): void { this.router.navigate(['/app/compras/proveedores', id]); }
  closeForm(): void { this.showForm.set(false); this.form.reset({ diasVencimiento: 30, descuentoProntoPago: 0 }); }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.submitting.set(true);
    const v = this.form.getRawValue();
    this.svc.createProveedor({
      razonSocial: v.razonSocial,
      nif: v.nif,
      email: v.email,
      direccion: v.direccion || undefined,
      ciudad: v.ciudad || undefined,
      codigoPostal: v.codigoPostal || undefined,
      pais: v.pais || undefined,
      telefono: v.telefono || undefined,
      web: v.web || undefined,
      diasVencimiento: v.diasVencimiento,
      descuentoProntoPago: v.descuentoProntoPago,
      metodoPago: v.metodoPago || undefined,
    } as any).subscribe({
      next: () => { this.notify.success('Proveedor creado correctamente'); this.closeForm(); this.submitting.set(false); this.load(); },
      error: (err) => { this.notify.error(err?.error?.message ?? 'Error al crear proveedor'); this.submitting.set(false); },
    });
  }

  private load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.svc.listProveedores(this.currentSearch || undefined, this.currentPage()).subscribe({
      next: (res) => { this.items.set(res.items); this.totalCount.set(res.totalCount); this.totalPages.set(res.totalPages); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar proveedores'); this.loading.set(false); },
    });
  }
}
