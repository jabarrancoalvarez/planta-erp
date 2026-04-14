import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { InventarioService, ProductoDetailDto } from '../../../core/services/inventario.service';

@Component({
  selector: 'app-producto-detail',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="detail-page">
      <div class="detail-page__header">
        <button class="detail-page__back" (click)="goBack()">&larr; Volver</button>
        <h1 class="detail-page__title">Detalle de Producto</h1>
        @if (item() && !editing()) {
          <div style="margin-left:auto; display:flex; gap:0.5rem;">
            <button class="detail-page__back" (click)="startEdit()">Editar</button>
            <button class="detail-page__back" style="background:#fee;color:#c00;" (click)="onDelete()">Eliminar</button>
          </div>
        }
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando producto...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else if (item(); as p) {
        @if (editing()) {
          <div class="detail-page__section">
            <h2 class="detail-page__section-title">Editar Producto</h2>
            <div class="detail-page__grid">
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="edit-nombre">Nombre</label>
                <input id="edit-nombre" type="text" [(ngModel)]="editNombre" />
              </div>
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="edit-precio-compra">Precio Compra</label>
                <input id="edit-precio-compra" type="number" step="0.01" [(ngModel)]="editPrecioCompra" />
              </div>
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="edit-precio-venta">Precio Venta</label>
                <input id="edit-precio-venta" type="number" step="0.01" [(ngModel)]="editPrecioVenta" />
              </div>
              <div class="detail-page__field" style="grid-column:1/-1;">
                <label class="detail-page__field-label" for="edit-descripcion">Descripcion</label>
                <textarea id="edit-descripcion" rows="3" [(ngModel)]="editDescripcion"></textarea>
              </div>
            </div>
            @if (saveError()) {
              <div class="error-state">{{ saveError() }}</div>
            }
            <div style="display:flex; gap:0.5rem; margin-top:1rem;">
              <button class="detail-page__back" (click)="saveEdit()" [disabled]="saving()">
                {{ saving() ? 'Guardando...' : 'Guardar' }}
              </button>
              <button class="detail-page__back" (click)="cancelEdit()">Cancelar</button>
            </div>
          </div>
        } @else {
          <div class="detail-page__section">
            <h2 class="detail-page__section-title">Informacion General</h2>
            <div class="detail-page__grid">
              <div class="detail-page__field">
                <span class="detail-page__field-label">SKU</span>
                <span class="detail-page__field-value"><code>{{ p.sku }}</code></span>
              </div>
              <div class="detail-page__field">
                <span class="detail-page__field-label">Nombre</span>
                <span class="detail-page__field-value">{{ p.nombre }}</span>
              </div>
              <div class="detail-page__field">
                <span class="detail-page__field-label">Tipo</span>
                <span class="detail-page__field-value">{{ p.tipo }}</span>
              </div>
              <div class="detail-page__field">
                <span class="detail-page__field-label">Unidad Medida</span>
                <span class="detail-page__field-value">{{ p.unidadMedida }}</span>
              </div>
              <div class="detail-page__field">
                <span class="detail-page__field-label">Estado</span>
                <span class="detail-page__field-value">
                  <span class="active-dot" [class.active-dot--active]="p.activo" [class.active-dot--inactive]="!p.activo"></span>
                  {{ p.activo ? 'Activo' : 'Inactivo' }}
                </span>
              </div>
              <div class="detail-page__field">
                <span class="detail-page__field-label">Fecha Creacion</span>
                <span class="detail-page__field-value">{{ p.createdAt | date:'dd/MM/yyyy HH:mm' }}</span>
              </div>
            </div>
          </div>

          <div class="detail-page__section">
            <h2 class="detail-page__section-title">Precios y Peso</h2>
            <div class="detail-page__grid">
              <div class="detail-page__field">
                <span class="detail-page__field-label">Precio Compra</span>
                <span class="detail-page__field-value">{{ p.precioCompra | number:'1.2-2' }} &euro;</span>
              </div>
              <div class="detail-page__field">
                <span class="detail-page__field-label">Precio Venta</span>
                <span class="detail-page__field-value">{{ p.precioVenta | number:'1.2-2' }} &euro;</span>
              </div>
              <div class="detail-page__field">
                <span class="detail-page__field-label">Peso</span>
                <span class="detail-page__field-value">{{ p.pesoKg }} kg</span>
              </div>
            </div>
          </div>

          @if (p.descripcion) {
            <div class="detail-page__section">
              <h2 class="detail-page__section-title">Descripcion</h2>
              <p>{{ p.descripcion }}</p>
            </div>
          }
        }
      }
    </div>
  `,
})
export class ProductoDetailComponent implements OnInit {
  private svc = inject(InventarioService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  readonly item = signal<ProductoDetailDto | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly editing = signal(false);
  readonly saving = signal(false);
  readonly saveError = signal<string | null>(null);

  editNombre = '';
  editDescripcion = '';
  editPrecioCompra = 0;
  editPrecioVenta = 0;

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.load(id);
  }

  private load(id: string): void {
    this.loading.set(true);
    this.svc.getProducto(id).subscribe({
      next: (data) => { this.item.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar producto'); this.loading.set(false); },
    });
  }

  startEdit(): void {
    const p = this.item();
    if (!p) return;
    this.editNombre = p.nombre;
    this.editDescripcion = p.descripcion ?? '';
    this.editPrecioCompra = p.precioCompra;
    this.editPrecioVenta = p.precioVenta;
    this.saveError.set(null);
    this.editing.set(true);
  }

  cancelEdit(): void {
    this.editing.set(false);
    this.saveError.set(null);
  }

  saveEdit(): void {
    const p = this.item();
    if (!p) return;
    this.saving.set(true);
    this.saveError.set(null);
    this.svc.updateProducto(p.id, {
      nombre: this.editNombre,
      descripcion: this.editDescripcion || undefined,
      precioCompra: this.editPrecioCompra,
      precioVenta: this.editPrecioVenta,
    }).subscribe({
      next: () => {
        this.saving.set(false);
        this.editing.set(false);
        this.load(p.id);
      },
      error: (err) => {
        this.saveError.set(err?.error?.message ?? 'Error al guardar');
        this.saving.set(false);
      },
    });
  }

  onDelete(): void {
    const p = this.item();
    if (!p) return;
    if (!confirm(`¿Eliminar producto "${p.nombre}"? Esta accion no se puede deshacer.`)) return;
    this.svc.deleteProducto(p.id).subscribe({
      next: () => this.router.navigate(['/app/inventario/productos']),
      error: (err) => this.error.set(err?.error?.message ?? 'Error al eliminar'),
    });
  }

  goBack(): void {
    this.router.navigate(['/app/inventario/productos']);
  }
}
