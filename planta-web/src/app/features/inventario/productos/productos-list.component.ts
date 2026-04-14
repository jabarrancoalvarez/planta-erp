import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { InventarioService, ProductoListDto } from '../../../core/services/inventario.service';
import { NotificationService } from '../../../shared/components/toast/notification.service';
import { ExportService } from '../../../core/services/export.service';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-productos-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ReactiveFormsModule, PaginationComponent],
  template: `
    <div class="page">
      <div class="page__header">
        <div>
          <h1 class="page__title">Productos</h1>
          <p class="page__subtitle">Gestion de productos del inventario</p>
        </div>
        <div class="page__actions">
          <button class="btn-outline" (click)="exportCsv()">Exportar CSV</button>
          <button class="btn-primary" (click)="showForm.set(true)">+ Nuevo Producto</button>
        </div>
      </div>

      <div class="page__filters">
        <input class="filter-input" placeholder="Buscar por nombre o SKU..."
               (input)="onSearch($event)" />
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando productos...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>SKU</th>
                <th>Nombre</th>
                <th>Tipo</th>
                <th>Unidad</th>
                <th>Precio Venta</th>
                <th>Estado</th>
              </tr>
            </thead>
            <tbody>
              @for (item of items(); track item.id) {
                <tr class="clickable-row" (click)="goToDetail(item.id)">
                  <td><code>{{ item.sku }}</code></td>
                  <td>{{ item.nombre }}</td>
                  <td>{{ item.tipo }}</td>
                  <td>{{ item.unidadMedida }}</td>
                  <td>{{ item.precioVenta | number:'1.2-2' }} &euro;</td>
                  <td>
                    <span class="active-dot" [class.active-dot--active]="item.activo" [class.active-dot--inactive]="!item.activo"></span>
                    {{ item.activo ? 'Activo' : 'Inactivo' }}
                  </td>
                </tr>
              } @empty {
                <tr><td colspan="6" class="empty-state">No se encontraron resultados</td></tr>
              }
            </tbody>
          </table>
        </div>

        @if (totalPages() > 1) {
          <app-pagination
            [currentPage]="currentPage()"
            [totalPages]="totalPages()"
            [totalCount]="totalCount()"
            (pageChange)="onPageChange($event)" />
        }

        <div class="page__footer">{{ items().length }} de {{ totalCount() }} registros</div>
      }
    </div>

    @if (showForm()) {
      <div class="form-overlay" (click)="closeForm()">
        <div class="form-dialog" (click)="$event.stopPropagation()">
          <h2 class="form-dialog__title">Nuevo Producto</h2>
          <form [formGroup]="form" (ngSubmit)="onSubmit()">
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">SKU *</label>
                <input class="form-input" formControlName="sku" placeholder="PRD-001" />
              </div>
              <div class="form-group">
                <label class="form-label">Nombre *</label>
                <input class="form-input" formControlName="nombre" />
              </div>
            </div>
            <div class="form-group">
              <label class="form-label">Descripcion</label>
              <textarea class="form-textarea" formControlName="descripcion"></textarea>
            </div>
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Tipo *</label>
                <select class="form-select" formControlName="tipo">
                  <option value="">Seleccionar...</option>
                  <option value="MateriaPrima">Materia Prima</option>
                  <option value="Semielaborado">Semielaborado</option>
                  <option value="ProductoTerminado">Producto Terminado</option>
                </select>
              </div>
              <div class="form-group">
                <label class="form-label">Unidad Medida *</label>
                <select class="form-select" formControlName="unidadMedida">
                  <option value="">Seleccionar...</option>
                  <option value="Kilogramo">Kilogramo (kg)</option>
                  <option value="Unidad">Unidad (ud)</option>
                  <option value="Metro">Metro (m)</option>
                  <option value="Litro">Litro (l)</option>
                  <option value="Gramo">Gramo (g)</option>
                  <option value="Mililitro">Mililitro (ml)</option>
                </select>
              </div>
            </div>
            <div class="form-row">
              <div class="form-group">
                <label class="form-label">Precio Compra</label>
                <input class="form-input" type="number" step="0.01" formControlName="precioCompra" />
              </div>
              <div class="form-group">
                <label class="form-label">Precio Venta</label>
                <input class="form-input" type="number" step="0.01" formControlName="precioVenta" />
              </div>
            </div>
            <div class="form-group">
              <label class="form-label">Peso (kg)</label>
              <input class="form-input" type="number" step="0.01" formControlName="pesoKg" />
            </div>
            <div class="form-actions">
              <button type="button" class="btn-outline" (click)="closeForm()">Cancelar</button>
              <button type="submit" class="btn-primary" [disabled]="form.invalid || submitting()">
                {{ submitting() ? 'Creando...' : 'Crear Producto' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    }
  `,
})
export class ProductosListComponent implements OnInit {
  private svc = inject(InventarioService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private notify = inject(NotificationService);
  private exportSvc = inject(ExportService);

  readonly items = signal<ProductoListDto[]>([]);
  readonly totalCount = signal(0);
  readonly currentPage = signal(1);
  readonly totalPages = signal(1);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly showForm = signal(false);
  readonly submitting = signal(false);
  private currentSearch = '';
  private searchTimeout: any;

  readonly form = this.fb.nonNullable.group({
    sku: ['', Validators.required],
    nombre: ['', Validators.required],
    descripcion: [''],
    tipo: ['', Validators.required],
    unidadMedida: ['', Validators.required],
    precioCompra: [0],
    precioVenta: [0],
    pesoKg: [0],
  });

  ngOnInit(): void {
    this.load();
  }

  onSearch(event: Event): void {
    clearTimeout(this.searchTimeout);
    this.currentSearch = (event.target as HTMLInputElement).value;
    this.searchTimeout = setTimeout(() => { this.currentPage.set(1); this.load(); }, 300);
  }

  onPageChange(page: number): void {
    this.currentPage.set(page);
    this.load();
  }

  goToDetail(id: string): void {
    this.router.navigate(['/app/inventario/productos', id]);
  }

  exportCsv(): void {
    const headers = ['SKU', 'Nombre', 'Tipo', 'Unidad', 'Precio Venta', 'Activo'];
    const rows = this.items().map(i => [i.sku, i.nombre, i.tipo, i.unidadMedida, i.precioVenta, i.activo ? 'Si' : 'No']);
    this.exportSvc.exportToCsv('productos', headers, rows);
  }

  closeForm(): void {
    this.showForm.set(false);
    this.form.reset();
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.submitting.set(true);
    const val = this.form.getRawValue();
    this.svc.createProducto({
      sku: val.sku,
      nombre: val.nombre,
      descripcion: val.descripcion || undefined,
      tipo: val.tipo,
      unidadMedida: val.unidadMedida,
      precioCompra: val.precioCompra,
      precioVenta: val.precioVenta,
      pesoKg: val.pesoKg,
    } as any).subscribe({
      next: () => {
        this.notify.success('Producto creado correctamente');
        this.closeForm();
        this.submitting.set(false);
        this.load();
      },
      error: (err) => {
        this.notify.error(err?.error?.message ?? 'Error al crear producto');
        this.submitting.set(false);
      },
    });
  }

  private load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.svc.listProductos(this.currentSearch || undefined, this.currentPage()).subscribe({
      next: (res) => {
        this.items.set(res.items);
        this.totalCount.set(res.totalCount);
        this.totalPages.set(res.totalPages);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err?.error?.message ?? 'Error al cargar productos');
        this.loading.set(false);
      },
    });
  }
}
