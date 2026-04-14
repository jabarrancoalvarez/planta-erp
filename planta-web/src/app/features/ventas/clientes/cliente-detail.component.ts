import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { VentasService, ClienteDetailDto } from '../../../core/services/ventas.service';

@Component({
  selector: 'app-cliente-detail',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="detail-page">
      <div class="detail-page__header">
        <button class="detail-page__back" (click)="goBack()">&larr; Volver</button>
        <h1 class="detail-page__title">Detalle de Cliente</h1>
        @if (item() && !editing()) {
          <div style="margin-left:auto; display:flex; gap:0.5rem;">
            <button class="detail-page__back" (click)="startEdit()">Editar</button>
            <button class="detail-page__back" style="background:#fee;color:#c00;" (click)="onDelete()">Eliminar</button>
          </div>
        }
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando cliente...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else if (item(); as c) {
        @if (editing()) {
          <div class="detail-page__section">
            <h2 class="detail-page__section-title">Editar Cliente</h2>
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
                <label class="detail-page__field-label" for="ed-envio">Dirección Envío</label>
                <input id="ed-envio" type="text" [(ngModel)]="edDireccionEnvio" />
              </div>
              <div class="detail-page__field">
                <label class="detail-page__field-label" for="ed-fact">Dirección Facturación</label>
                <input id="ed-fact" type="text" [(ngModel)]="edDireccionFacturacion" />
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
                <span class="detail-page__field-value">{{ c.razonSocial }}</span>
              </div>
              <div class="detail-page__field">
                <span class="detail-page__field-label">NIF</span>
                <span class="detail-page__field-value"><code>{{ c.nif }}</code></span>
              </div>
              <div class="detail-page__field">
                <span class="detail-page__field-label">Email</span>
                <span class="detail-page__field-value">{{ c.email }}</span>
              </div>
              <div class="detail-page__field">
                <span class="detail-page__field-label">Telefono</span>
                <span class="detail-page__field-value">{{ c.telefono ?? '---' }}</span>
              </div>
              <div class="detail-page__field">
                <span class="detail-page__field-label">Web</span>
                <span class="detail-page__field-value">{{ c.web ?? '---' }}</span>
              </div>
              <div class="detail-page__field">
                <span class="detail-page__field-label">Estado</span>
                <span class="detail-page__field-value">
                  <span class="active-dot" [class.active-dot--active]="c.activo" [class.active-dot--inactive]="!c.activo"></span>
                  {{ c.activo ? 'Activo' : 'Inactivo' }}
                </span>
              </div>
            </div>
          </div>

          <div class="detail-page__section">
            <h2 class="detail-page__section-title">Direcciones</h2>
            <div class="detail-page__grid">
              <div class="detail-page__field">
                <span class="detail-page__field-label">Direccion Envio</span>
                <span class="detail-page__field-value">{{ c.direccionEnvio ?? '---' }}</span>
              </div>
              <div class="detail-page__field">
                <span class="detail-page__field-label">Direccion Facturacion</span>
                <span class="detail-page__field-value">{{ c.direccionFacturacion ?? '---' }}</span>
              </div>
              <div class="detail-page__field">
                <span class="detail-page__field-label">Ciudad</span>
                <span class="detail-page__field-value">{{ c.ciudad ?? '---' }}</span>
              </div>
              <div class="detail-page__field">
                <span class="detail-page__field-label">Codigo Postal</span>
                <span class="detail-page__field-value">{{ c.codigoPostal ?? '---' }}</span>
              </div>
              <div class="detail-page__field">
                <span class="detail-page__field-label">Pais</span>
                <span class="detail-page__field-value">{{ c.pais ?? '---' }}</span>
              </div>
            </div>
          </div>
        }
      }
    </div>
  `,
})
export class ClienteDetailComponent implements OnInit {
  private svc = inject(VentasService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  readonly item = signal<ClienteDetailDto | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly editing = signal(false);
  readonly saving = signal(false);
  readonly saveError = signal<string | null>(null);

  edRazon = '';
  edEmail = '';
  edTelefono = '';
  edWeb = '';
  edDireccionEnvio = '';
  edDireccionFacturacion = '';
  edCiudad = '';
  edCodigoPostal = '';
  edPais = '';

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.load(id);
  }

  private load(id: string): void {
    this.loading.set(true);
    this.svc.getCliente(id).subscribe({
      next: (data) => { this.item.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar cliente'); this.loading.set(false); },
    });
  }

  startEdit(): void {
    const c = this.item();
    if (!c) return;
    this.edRazon = c.razonSocial;
    this.edEmail = c.email;
    this.edTelefono = c.telefono ?? '';
    this.edWeb = c.web ?? '';
    this.edDireccionEnvio = c.direccionEnvio ?? '';
    this.edDireccionFacturacion = c.direccionFacturacion ?? '';
    this.edCiudad = c.ciudad ?? '';
    this.edCodigoPostal = c.codigoPostal ?? '';
    this.edPais = c.pais ?? '';
    this.saveError.set(null);
    this.editing.set(true);
  }

  cancelEdit(): void {
    this.editing.set(false);
    this.saveError.set(null);
  }

  saveEdit(): void {
    const c = this.item();
    if (!c) return;
    this.saving.set(true);
    this.saveError.set(null);
    this.svc.updateCliente(c.id, {
      razonSocial: this.edRazon,
      email: this.edEmail,
      telefono: this.edTelefono || null,
      web: this.edWeb || null,
      direccionEnvio: this.edDireccionEnvio || null,
      direccionFacturacion: this.edDireccionFacturacion || null,
      ciudad: this.edCiudad || null,
      codigoPostal: this.edCodigoPostal || null,
      pais: this.edPais || null,
    }).subscribe({
      next: () => {
        this.saving.set(false);
        this.editing.set(false);
        this.load(c.id);
      },
      error: (err) => {
        this.saveError.set(err?.error?.message ?? 'Error al guardar');
        this.saving.set(false);
      },
    });
  }

  goBack(): void { this.router.navigate(['/app/ventas/clientes']); }

  onDelete(): void {
    const c = this.item();
    if (!c) return;
    if (!confirm(`¿Eliminar cliente "${c.razonSocial}"?`)) return;
    this.svc.deleteCliente(c.id).subscribe({
      next: () => this.router.navigate(['/app/ventas/clientes']),
      error: (err) => this.error.set(err?.error?.message ?? 'Error al eliminar'),
    });
  }
}
