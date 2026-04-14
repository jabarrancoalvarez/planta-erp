import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule, DatePipe, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { FacturacionService, FacturaListDto, EstadoFactura } from '../../core/services/facturacion.service';
import { VentasService } from '../../core/services/ventas.service';

@Component({
  selector: 'app-facturas-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule, DatePipe, DecimalPipe],
  template: `
    <div class="list-page">
      <div class="list-page__header">
        <h1 class="list-page__title">Facturas</h1>
        <button class="btn-primary" (click)="toggleCreate()">{{ showCreate() ? 'Cancelar' : '+ Nueva factura' }}</button>
      </div>

      @if (showCreate()) {
        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Crear factura</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <label class="detail-page__field-label">Serie *</label>
              <input [(ngModel)]="newSerie" placeholder="A" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Cliente *</label>
              <select [(ngModel)]="newClienteId" (ngModelChange)="onClienteChange($event)">
                <option value="">--</option>
                @for (c of clientes(); track c.id) {
                  <option [value]="c.id">{{ c.razonSocial }}</option>
                }
              </select>
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Descripcion linea *</label>
              <input [(ngModel)]="newDesc" placeholder="Servicio prestado" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Cantidad</label>
              <input type="number" [(ngModel)]="newCantidad" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Precio unitario</label>
              <input type="number" [(ngModel)]="newPrecio" />
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">IVA %</label>
              <input type="number" [(ngModel)]="newIva" />
            </div>
          </div>
          @if (createError()) { <div class="error-state">{{ createError() }}</div> }
          <div style="display:flex; gap:0.5rem; margin-top:1rem;">
            <button class="btn-primary" (click)="save()" [disabled]="saving()">{{ saving() ? 'Creando...' : 'Crear' }}</button>
          </div>
        </div>
      }

      <div class="filters-bar">
        <select [(ngModel)]="filterEstado" (ngModelChange)="load()">
          <option value="">Todos los estados</option>
          @for (e of estados; track e) { <option [value]="e">{{ e }}</option> }
        </select>
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Numero</th>
                <th>Cliente</th>
                <th>Fecha</th>
                <th style="text-align:right">Total</th>
                <th>Estado</th>
                <th>Verifactu</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              @for (f of items(); track f.id) {
                <tr>
                  <td><code>{{ f.numeroCompleto }}</code></td>
                  <td>{{ f.clienteNombre }}</td>
                  <td>{{ f.fechaEmision | date:'dd/MM/yyyy' }}</td>
                  <td style="text-align:right">{{ f.total | number:'1.2-2' }} &euro;</td>
                  <td><span class="badge">{{ f.estado }}</span></td>
                  <td><span class="badge badge--neutral">{{ f.estadoVerifactu }}</span></td>
                  <td>
                    @if (f.estado === 'Borrador') {
                      <button class="btn-sm" (click)="emitir(f)">Emitir</button>
                      <button class="btn-outline btn-sm" style="background:#fee;color:#c00;" (click)="remove(f)">Eliminar</button>
                    }
                    @if (f.estado === 'Emitida' || f.estado === 'Firmada') {
                      <button class="btn-sm" (click)="verifactu(f)">Verifactu</button>
                    }
                  </td>
                </tr>
              } @empty {
                <tr><td colspan="7" class="empty-state">Sin facturas</td></tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `,
})
export class FacturasListComponent implements OnInit {
  private svc = inject(FacturacionService);
  private ventas = inject(VentasService);
  private router = inject(Router);

  readonly items = signal<FacturaListDto[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly showCreate = signal(false);
  readonly saving = signal(false);
  readonly createError = signal<string | null>(null);
  readonly clientes = signal<{ id: string; razonSocial: string }[]>([]);

  estados: EstadoFactura[] = ['Borrador', 'Emitida', 'Firmada', 'EnviadaVerifactu', 'Aceptada', 'Rechazada', 'Anulada'];
  filterEstado = '';

  newSerie = 'A';
  newClienteId = '';
  newClienteNombre = '';
  newDesc = '';
  newCantidad = 1;
  newPrecio = 0;
  newIva = 21;

  ngOnInit(): void {
    this.load();
    this.ventas.listClientes(undefined, 1, 100).subscribe({
      next: (r) => this.clientes.set(r.items.map(c => ({ id: c.id, razonSocial: c.razonSocial }))),
      error: () => {},
    });
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.svc.listFacturas({ estado: (this.filterEstado as EstadoFactura) || undefined }).subscribe({
      next: (r) => { this.items.set(r.items); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar facturas'); this.loading.set(false); },
    });
  }

  toggleCreate(): void {
    this.showCreate.update(v => !v);
    this.createError.set(null);
  }

  onClienteChange(id: string): void {
    const c = this.clientes().find(x => x.id === id);
    this.newClienteNombre = c?.razonSocial ?? '';
  }

  save(): void {
    if (!this.newClienteId || !this.newDesc) {
      this.createError.set('Cliente y descripcion obligatorios');
      return;
    }
    this.saving.set(true);
    this.createError.set(null);
    this.svc.createFactura({
      serieCodigo: this.newSerie,
      clienteId: this.newClienteId,
      clienteNombre: this.newClienteNombre,
      lineas: [{ descripcion: this.newDesc, cantidad: this.newCantidad, precioUnitario: this.newPrecio, ivaPct: this.newIva }],
    }).subscribe({
      next: () => {
        this.saving.set(false);
        this.showCreate.set(false);
        this.newDesc = ''; this.newCantidad = 1; this.newPrecio = 0;
        this.load();
      },
      error: (err) => {
        this.createError.set(err?.error?.message ?? 'Error al crear factura');
        this.saving.set(false);
      },
    });
  }

  emitir(f: FacturaListDto): void {
    this.svc.emitir(f.id).subscribe({ next: () => this.load(), error: (e) => this.error.set(e?.error?.message ?? 'Error emitir') });
  }

  verifactu(f: FacturaListDto): void {
    this.svc.enviarVerifactu(f.id).subscribe({ next: () => this.load(), error: (e) => this.error.set(e?.error?.message ?? 'Error verifactu') });
  }

  remove(f: FacturaListDto): void {
    if (!confirm(`¿Eliminar factura ${f.numeroCompleto}?`)) return;
    this.svc.deleteFactura(f.id).subscribe({
      next: () => this.load(),
      error: (err) => this.error.set(err?.error?.message ?? 'Error al eliminar'),
    });
  }
}
