import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ComprasService, OCDetailDto } from '../../../core/services/compras.service';
import { NotificationService } from '../../../shared/components/toast/notification.service';

@Component({
  selector: 'app-oc-detail',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="detail-page">
      <div class="detail-page__header">
        <button class="detail-page__back" (click)="goBack()">&larr; Volver</button>
        <h1 class="detail-page__title">Orden de Compra</h1>
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando orden...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else if (item(); as oc) {
        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Informacion General</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Codigo</span>
              <span class="detail-page__field-value"><code>{{ oc.codigo }}</code></span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Proveedor</span>
              <span class="detail-page__field-value">{{ oc.proveedorRazonSocial }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Estado</span>
              <span class="detail-page__field-value">
                <span class="badge"
                  [class.badge--warning]="oc.estadoOC === 'Pendiente' || oc.estadoOC === 'Borrador'"
                  [class.badge--info]="oc.estadoOC === 'Enviada'"
                  [class.badge--success]="oc.estadoOC === 'Recibida'"
                  [class.badge--neutral]="oc.estadoOC === 'Cancelada'">
                  {{ oc.estadoOC }}
                </span>
              </span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Fecha Emision</span>
              <span class="detail-page__field-value">{{ oc.fechaEmision | date:'dd/MM/yyyy' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Fecha Entrega Est.</span>
              <span class="detail-page__field-value">{{ oc.fechaEntregaEstimada ? (oc.fechaEntregaEstimada | date:'dd/MM/yyyy') : '---' }}</span>
            </div>
          </div>
        </div>

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Totales</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Subtotal</span>
              <span class="detail-page__field-value">{{ oc.subtotal | number:'1.2-2' }} &euro;</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Impuestos</span>
              <span class="detail-page__field-value">{{ oc.impuestos | number:'1.2-2' }} &euro;</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Total</span>
              <span class="detail-page__field-value"><strong>{{ oc.total | number:'1.2-2' }} &euro;</strong></span>
            </div>
          </div>
        </div>

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Lineas ({{ oc.lineas.length }})</h2>
          @if (oc.lineas.length > 0) {
            <div class="table-container">
              <table class="data-table">
                <thead>
                  <tr>
                    <th>Producto</th>
                    <th>Cantidad</th>
                    <th>Precio Unitario</th>
                    <th>Total</th>
                  </tr>
                </thead>
                <tbody>
                  @for (line of oc.lineas; track line.id) {
                    <tr>
                      <td>{{ line.productoNombre }}</td>
                      <td>{{ line.cantidad }}</td>
                      <td>{{ line.precioUnitario | number:'1.2-2' }} &euro;</td>
                      <td>{{ line.total | number:'1.2-2' }} &euro;</td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          } @else {
            <p class="empty-state">Sin lineas definidas</p>
          }
        </div>

        @if (oc.observaciones) {
          <div class="detail-page__section">
            <h2 class="detail-page__section-title">Observaciones</h2>
            <p>{{ oc.observaciones }}</p>
          </div>
        }

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Cambiar Estado</h2>
          <div class="detail-page__actions">
            @for (transition of getAvailableTransitions(oc.estadoOC); track transition) {
              <button class="btn-outline btn-sm" [class.btn-danger]="transition === 'Cancelada'" (click)="changeState(transition)">
                {{ transition }}
              </button>
            }
          </div>
        </div>
      }
    </div>
  `,
})
export class OCDetailComponent implements OnInit {
  private svc = inject(ComprasService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private notify = inject(NotificationService);

  readonly item = signal<OCDetailDto | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  private id = '';

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id')!;
    this.loadDetail();
  }

  goBack(): void { this.router.navigate(['/app/compras/ordenes']); }

  getAvailableTransitions(state: string): string[] {
    const map: Record<string, string[]> = {
      'Borrador': ['Pendiente', 'Cancelada'],
      'Pendiente': ['Enviada', 'Cancelada'],
      'Enviada': ['Recibida', 'Cancelada'],
    };
    return map[state] ?? [];
  }

  changeState(estadoDestino: string): void {
    this.svc.cambiarEstadoOC(this.id, estadoDestino).subscribe({
      next: () => { this.notify.success(`Estado cambiado a ${estadoDestino}`); this.loadDetail(); },
      error: (err) => this.notify.error(err?.error?.message ?? 'Error al cambiar estado'),
    });
  }

  private loadDetail(): void {
    this.loading.set(true);
    this.svc.getOC(this.id).subscribe({
      next: (data) => { this.item.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar orden'); this.loading.set(false); },
    });
  }
}
