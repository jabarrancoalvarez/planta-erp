import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { VentasService, PedidoDetailDto } from '../../../core/services/ventas.service';
import { NotificationService } from '../../../shared/components/toast/notification.service';

@Component({
  selector: 'app-pedido-detail',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="detail-page">
      <div class="detail-page__header">
        <button class="detail-page__back" (click)="goBack()">&larr; Volver</button>
        <h1 class="detail-page__title">Pedido de Venta</h1>
        @if (item() && !editing()) {
          <button class="detail-page__back" style="margin-left:auto;" (click)="startEdit()">Editar</button>
          <button class="detail-page__back" style="background:#fee;color:#c00;" (click)="onDelete()">Eliminar</button>
        }
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando pedido...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else if (item(); as ped) {
        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Informacion General</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Codigo</span>
              <span class="detail-page__field-value"><code>{{ ped.codigo }}</code></span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Cliente</span>
              <span class="detail-page__field-value">{{ ped.clienteRazonSocial }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Estado</span>
              <span class="detail-page__field-value">
                <span class="badge"
                  [class.badge--info]="ped.estadoPedido === 'Confirmado'"
                  [class.badge--warning]="ped.estadoPedido === 'EnPreparacion' || ped.estadoPedido === 'Borrador'"
                  [class.badge--success]="ped.estadoPedido === 'Expedido' || ped.estadoPedido === 'Entregado'"
                  [class.badge--neutral]="ped.estadoPedido === 'Cancelado'">
                  {{ ped.estadoPedido }}
                </span>
              </span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Fecha Emision</span>
              <span class="detail-page__field-value">{{ ped.fechaEmision | date:'dd/MM/yyyy' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Fecha Entrega Est.</span>
              <span class="detail-page__field-value">{{ ped.fechaEntregaEstimada ? (ped.fechaEntregaEstimada | date:'dd/MM/yyyy') : '---' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Direccion Entrega</span>
              <span class="detail-page__field-value">{{ ped.direccionEntrega ?? '---' }}</span>
            </div>
          </div>
        </div>

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Totales</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Subtotal</span>
              <span class="detail-page__field-value">{{ ped.subtotal | number:'1.2-2' }} &euro;</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Impuestos</span>
              <span class="detail-page__field-value">{{ ped.impuestos | number:'1.2-2' }} &euro;</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Total</span>
              <span class="detail-page__field-value"><strong>{{ ped.total | number:'1.2-2' }} &euro;</strong></span>
            </div>
          </div>
        </div>

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Lineas ({{ ped.lineas.length }})</h2>
          @if (ped.lineas.length > 0) {
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
                  @for (line of ped.lineas; track line.id) {
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

        @if (editing()) {
          <div class="detail-page__section">
            <h2 class="detail-page__section-title">Editar</h2>
            <div class="detail-page__grid">
              <div class="detail-page__field">
                <label class="detail-page__field-label">Fecha Entrega Estimada</label>
                <input type="date" [(ngModel)]="editFecha" />
              </div>
              <div class="detail-page__field">
                <label class="detail-page__field-label">Direccion Entrega</label>
                <input [(ngModel)]="editDireccion" />
              </div>
              <div class="detail-page__field">
                <label class="detail-page__field-label">Observaciones</label>
                <input [(ngModel)]="editObservaciones" />
              </div>
            </div>
            <div style="display:flex; gap:0.5rem; margin-top:1rem;">
              <button class="btn-primary" (click)="saveEdit()" [disabled]="savingEdit()">{{ savingEdit() ? 'Guardando...' : 'Guardar' }}</button>
              <button class="btn-outline" (click)="cancelEdit()">Cancelar</button>
            </div>
          </div>
        }

        @if (!editing() && ped.observaciones) {
          <div class="detail-page__section">
            <h2 class="detail-page__section-title">Observaciones</h2>
            <p>{{ ped.observaciones }}</p>
          </div>
        }

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Cambiar Estado</h2>
          <div class="detail-page__actions">
            @for (transition of getAvailableTransitions(ped.estadoPedido); track transition) {
              <button class="btn-outline btn-sm" [class.btn-danger]="transition === 'Cancelado'" (click)="changeState(transition)">
                {{ transition }}
              </button>
            }
          </div>
        </div>
      }
    </div>
  `,
})
export class PedidoDetailComponent implements OnInit {
  private svc = inject(VentasService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private notify = inject(NotificationService);

  readonly item = signal<PedidoDetailDto | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly editing = signal(false);
  readonly savingEdit = signal(false);
  editFecha = '';
  editDireccion = '';
  editObservaciones = '';
  private id = '';

  startEdit(): void {
    const p = this.item();
    if (!p) return;
    this.editFecha = p.fechaEntregaEstimada ? new Date(p.fechaEntregaEstimada).toISOString().slice(0, 10) : '';
    this.editDireccion = p.direccionEntrega ?? '';
    this.editObservaciones = p.observaciones ?? '';
    this.editing.set(true);
  }

  cancelEdit(): void { this.editing.set(false); }

  saveEdit(): void {
    this.savingEdit.set(true);
    this.svc.updatePedido(this.id, {
      fechaEntregaEstimada: this.editFecha ? new Date(this.editFecha).toISOString() : null,
      direccionEntrega: this.editDireccion || null,
      observaciones: this.editObservaciones || null,
    }).subscribe({
      next: () => {
        this.savingEdit.set(false);
        this.editing.set(false);
        this.notify.success('Pedido actualizado');
        this.loadDetail();
      },
      error: (err) => {
        this.savingEdit.set(false);
        this.notify.error(err?.error?.message ?? 'Error al actualizar');
      },
    });
  }

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id')!;
    this.loadDetail();
  }

  goBack(): void { this.router.navigate(['/app/ventas/pedidos']); }

  getAvailableTransitions(state: string): string[] {
    const map: Record<string, string[]> = {
      'Borrador': ['Confirmado', 'Cancelado'],
      'Confirmado': ['EnPreparacion', 'Cancelado'],
      'EnPreparacion': ['ParcialmenteEnviado', 'Enviado', 'Cancelado'],
      'ParcialmenteEnviado': ['Enviado', 'Cancelado'],
      'Enviado': ['Entregado'],
    };
    return map[state] ?? [];
  }

  changeState(estadoDestino: string): void {
    this.svc.cambiarEstadoPedido(this.id, estadoDestino).subscribe({
      next: () => { this.notify.success(`Estado cambiado a ${estadoDestino}`); this.loadDetail(); },
      error: (err) => this.notify.error(err?.error?.message ?? 'Error al cambiar estado'),
    });
  }

  onDelete(): void {
    const ped = this.item();
    if (!ped) return;
    if (!confirm(`¿Eliminar pedido "${ped.codigo}"?`)) return;
    this.svc.deletePedido(this.id).subscribe({
      next: () => this.router.navigate(['/app/ventas/pedidos']),
      error: (err) => this.notify.error(err?.error?.message ?? 'Error al eliminar'),
    });
  }

  private loadDetail(): void {
    this.loading.set(true);
    this.svc.getPedido(this.id).subscribe({
      next: (data) => { this.item.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar pedido'); this.loading.set(false); },
    });
  }
}
