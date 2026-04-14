import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ComprasService, ProveedorDetailDto } from '../../../core/services/compras.service';

@Component({
  selector: 'app-proveedor-detail',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="detail-page">
      <div class="detail-page__header">
        <button class="detail-page__back" (click)="goBack()">&larr; Volver</button>
        <h1 class="detail-page__title">Detalle de Proveedor</h1>
        @if (item() && !editing()) {
          <div style="margin-left:auto; display:flex; gap:0.5rem;">
            <button class="detail-page__back" (click)="startEdit()">Editar</button>
            <button class="detail-page__back" style="background:#fee;color:#c00;" (click)="onDelete()">Eliminar</button>
          </div>
        }
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando proveedor...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else if (item(); as p) {
        @if (editing()) {
          <div class="detail-page__section">
            <h2 class="detail-page__section-title">Editar Proveedor</h2>
            <div class="detail-page__grid">
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="ed-razon">Razón Social</label>
                <input id="ed-razon" type="text" [(ngModel)]="edRazon" />
              </div>
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="ed-email">Email</label>
                <input id="ed-email" type="email" [(ngModel)]="edEmail" />
              </div>
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="ed-telefono">Teléfono</label>
                <input id="ed-telefono" type="text" [(ngModel)]="edTelefono" />
              </div>
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="ed-web">Web</label>
                <input id="ed-web" type="text" [(ngModel)]="edWeb" />
              </div>
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="ed-direccion">Dirección</label>
                <input id="ed-direccion" type="text" [(ngModel)]="edDireccion" />
              </div>
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="ed-ciudad">Ciudad</label>
                <input id="ed-ciudad" type="text" [(ngModel)]="edCiudad" />
              </div>
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="ed-cp">Código Postal</label>
                <input id="ed-cp" type="text" [(ngModel)]="edCodigoPostal" />
              </div>
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="ed-pais">País</label>
                <input id="ed-pais" type="text" [(ngModel)]="edPais" />
              </div>
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="ed-dias">Días Vencimiento</label>
                <input id="ed-dias" type="number" [(ngModel)]="edDiasVencimiento" />
              </div>
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="ed-desc">Descuento Pronto Pago (%)</label>
                <input id="ed-desc" type="number" step="0.01" [(ngModel)]="edDescuento" />
              </div>
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="ed-pago">Método de Pago</label>
                <input id="ed-pago" type="text" [(ngModel)]="edMetodoPago" />
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
  readonly editing = signal(false);
  readonly saving = signal(false);
  readonly saveError = signal<string | null>(null);

  edRazon = '';
  edEmail = '';
  edTelefono = '';
  edWeb = '';
  edDireccion = '';
  edCiudad = '';
  edCodigoPostal = '';
  edPais = '';
  edDiasVencimiento = 0;
  edDescuento = 0;
  edMetodoPago = '';

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.load(id);
  }

  private load(id: string): void {
    this.loading.set(true);
    this.svc.getProveedor(id).subscribe({
      next: (data) => { this.item.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar proveedor'); this.loading.set(false); },
    });
  }

  startEdit(): void {
    const p = this.item();
    if (!p) return;
    this.edRazon = p.razonSocial;
    this.edEmail = p.email;
    this.edTelefono = p.telefono ?? '';
    this.edWeb = p.web ?? '';
    this.edDireccion = p.direccion ?? '';
    this.edCiudad = p.ciudad ?? '';
    this.edCodigoPostal = p.codigoPostal ?? '';
    this.edPais = p.pais ?? '';
    this.edDiasVencimiento = p.diasVencimiento;
    this.edDescuento = p.descuentoProntoPago;
    this.edMetodoPago = p.metodoPago ?? '';
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
    this.svc.updateProveedor(p.id, {
      razonSocial: this.edRazon,
      email: this.edEmail,
      telefono: this.edTelefono || null,
      web: this.edWeb || null,
      direccion: this.edDireccion || null,
      ciudad: this.edCiudad || null,
      codigoPostal: this.edCodigoPostal || null,
      pais: this.edPais || null,
      diasVencimiento: this.edDiasVencimiento,
      descuentoProntoPago: this.edDescuento,
      metodoPago: this.edMetodoPago || null,
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
