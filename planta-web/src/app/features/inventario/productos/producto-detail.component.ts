import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { InventarioService, ProductoDetailDto } from '../../../core/services/inventario.service';

@Component({
  selector: 'app-producto-detail',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="detail-page">
      <div class="detail-page__header">
        <button class="detail-page__back" (click)="goBack()">&larr; Volver</button>
        <h1 class="detail-page__title">Detalle de Producto</h1>
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando producto...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else if (item(); as p) {
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

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.loading.set(true);
    this.svc.getProducto(id).subscribe({
      next: (data) => { this.item.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar producto'); this.loading.set(false); },
    });
  }

  goBack(): void {
    this.router.navigate(['/app/inventario/productos']);
  }
}
