import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { VentasService, ClienteDetailDto } from '../../../core/services/ventas.service';

@Component({
  selector: 'app-cliente-detail',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="detail-page">
      <div class="detail-page__header">
        <button class="detail-page__back" (click)="goBack()">&larr; Volver</button>
        <h1 class="detail-page__title">Detalle de Cliente</h1>
        @if (item()) {
          <button class="detail-page__back" style="margin-left:auto;background:#fee;color:#c00;" (click)="onDelete()">Eliminar</button>
        }
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando cliente...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else if (item(); as c) {
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

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.loading.set(true);
    this.svc.getCliente(id).subscribe({
      next: (data) => { this.item.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar cliente'); this.loading.set(false); },
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
