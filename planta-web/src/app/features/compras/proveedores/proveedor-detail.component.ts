import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ComprasService, ProveedorDetailDto } from '../../../core/services/compras.service';

@Component({
  selector: 'app-proveedor-detail',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="detail-page">
      <div class="detail-page__header">
        <button class="detail-page__back" (click)="goBack()">&larr; Volver</button>
        <h1 class="detail-page__title">Detalle de Proveedor</h1>
        @if (item()) {
          <button class="detail-page__back" style="margin-left:auto;background:#fee;color:#c00;" (click)="onDelete()">Eliminar</button>
        }
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando proveedor...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else if (item(); as p) {
        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Datos Generales</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Razon Social</span>
              <span class="detail-page__field-value">{{ p.razonSocial }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">NIF</span>
              <span class="detail-page__field-value"><code>{{ p.nif }}</code></span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Email</span>
              <span class="detail-page__field-value">{{ p.email }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Telefono</span>
              <span class="detail-page__field-value">{{ p.telefono ?? '---' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Web</span>
              <span class="detail-page__field-value">{{ p.web ?? '---' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Estado</span>
              <span class="detail-page__field-value">
                <span class="active-dot" [class.active-dot--active]="p.activo" [class.active-dot--inactive]="!p.activo"></span>
                {{ p.activo ? 'Activo' : 'Inactivo' }}
              </span>
            </div>
          </div>
        </div>

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Direccion</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Direccion</span>
              <span class="detail-page__field-value">{{ p.direccion ?? '---' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Ciudad</span>
              <span class="detail-page__field-value">{{ p.ciudad ?? '---' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Codigo Postal</span>
              <span class="detail-page__field-value">{{ p.codigoPostal ?? '---' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Pais</span>
              <span class="detail-page__field-value">{{ p.pais ?? '---' }}</span>
            </div>
          </div>
        </div>

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Condiciones Comerciales</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Dias Vencimiento</span>
              <span class="detail-page__field-value">{{ p.diasVencimiento }} dias</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Descuento Pronto Pago</span>
              <span class="detail-page__field-value">{{ p.descuentoProntoPago }}%</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Metodo de Pago</span>
              <span class="detail-page__field-value">{{ p.metodoPago ?? '---' }}</span>
            </div>
          </div>
        </div>
      }
    </div>
  `,
})
export class ProveedorDetailComponent implements OnInit {
  private svc = inject(ComprasService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  readonly item = signal<ProveedorDetailDto | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.loading.set(true);
    this.svc.getProveedor(id).subscribe({
      next: (data) => { this.item.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar proveedor'); this.loading.set(false); },
    });
  }

  goBack(): void { this.router.navigate(['/app/compras/proveedores']); }

  onDelete(): void {
    const p = this.item();
    if (!p) return;
    if (!confirm(`¿Eliminar proveedor "${p.razonSocial}"?`)) return;
    this.svc.deleteProveedor(p.id).subscribe({
      next: () => this.router.navigate(['/app/compras/proveedores']),
      error: (err) => this.error.set(err?.error?.message ?? 'Error al eliminar'),
    });
  }
}
