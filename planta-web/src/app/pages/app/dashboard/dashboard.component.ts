import { Component, inject, signal, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { InventarioService } from '../../../core/services/inventario.service';
import { ProduccionService } from '../../../core/services/produccion.service';
import { ComprasService } from '../../../core/services/compras.service';
import { VentasService } from '../../../core/services/ventas.service';
import { CalidadService } from '../../../core/services/calidad.service';

interface KpiCard {
  label: string;
  value: string;
  delta: string;
  deltaPositive: boolean;
  icon: string;
  color: string;
}

interface ActividadReciente {
  tipo: 'inventario' | 'produccion' | 'compras' | 'ventas' | 'calidad';
  descripcion: string;
  fecha: string;
  usuario: string;
}

interface ModuloAcceso {
  nombre: string;
  descripcion: string;
  ruta: string;
  icon: string;
  color: string;
}

const MODULOS: ModuloAcceso[] = [
  { nombre: 'Inventario', descripcion: 'Productos, almacenes, lotes y movimientos', ruta: '/app/inventario', icon: 'package', color: '#2563eb' },
  { nombre: 'Produccion', descripcion: 'BOM, rutas y ordenes de fabricacion', ruta: '/app/produccion', icon: 'factory', color: '#f59e0b' },
  { nombre: 'Compras', descripcion: 'Proveedores, OC y recepciones', ruta: '/app/compras', icon: 'cart', color: '#ef4444' },
  { nombre: 'Ventas', descripcion: 'Clientes, pedidos y expediciones', ruta: '/app/ventas', icon: 'truck', color: '#10b981' },
  { nombre: 'Calidad', descripcion: 'Inspecciones y no conformidades', ruta: '/app/calidad', icon: 'check', color: '#8b5cf6' },
];

@Component({
  selector: 'app-dashboard',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  readonly auth = inject(AuthService);
  private inventarioSvc = inject(InventarioService);
  private produccionSvc = inject(ProduccionService);
  private comprasSvc = inject(ComprasService);
  private ventasSvc = inject(VentasService);
  private calidadSvc = inject(CalidadService);

  readonly kpis = signal<KpiCard[]>([]);
  readonly actividad = signal<ActividadReciente[]>([]);
  readonly modulos = signal<ModuloAcceso[]>(MODULOS);

  ngOnInit(): void {
    this.loadKpis();
  }

  firstName(): string {
    return this.auth.currentUser()?.name?.split(' ')[0] ?? '';
  }

  tipoColor(tipo: string): string {
    const map: Record<string, string> = {
      inventario: 'primary',
      produccion: 'warning',
      compras: 'danger',
      ventas: 'success',
      calidad: 'info',
    };
    return map[tipo] ?? 'primary';
  }

  tipoLabel(tipo: string): string {
    const map: Record<string, string> = {
      inventario: 'Inventario',
      produccion: 'Produccion',
      compras: 'Compras',
      ventas: 'Ventas',
      calidad: 'Calidad',
    };
    return map[tipo] ?? tipo;
  }

  private loadKpis(): void {
    forkJoin({
      productos: this.inventarioSvc.listProductos(undefined, 1, 1),
      alertas: this.inventarioSvc.listAlertas(),
      ofs: this.produccionSvc.listOFs(undefined, undefined, 1, 1),
      ocs: this.comprasSvc.listOCs(undefined, undefined, 1, 1),
      pedidos: this.ventasSvc.listPedidos(undefined, undefined, 1, 1),
      inspecciones: this.calidadSvc.listInspecciones(undefined, undefined, 1, 1),
    }).subscribe({
      next: (data) => {
        this.kpis.set([
          { label: 'Total Productos', value: String(data.productos.totalCount), delta: 'En inventario', deltaPositive: true, icon: 'package', color: '#2563eb' },
          { label: 'Ordenes Fabricacion', value: String(data.ofs.totalCount), delta: 'Total registradas', deltaPositive: true, icon: 'factory', color: '#f59e0b' },
          { label: 'Ordenes Compra', value: String(data.ocs.totalCount), delta: 'Total registradas', deltaPositive: true, icon: 'cart', color: '#ef4444' },
          { label: 'Pedidos Venta', value: String(data.pedidos.totalCount), delta: 'Total registrados', deltaPositive: true, icon: 'truck', color: '#10b981' },
          { label: 'Inspecciones', value: String(data.inspecciones.totalCount), delta: 'Total realizadas', deltaPositive: true, icon: 'check', color: '#8b5cf6' },
          { label: 'Alertas Stock', value: String(data.alertas.length), delta: data.alertas.length > 0 ? 'Requieren atencion' : 'Sin alertas', deltaPositive: data.alertas.length === 0, icon: 'alert', color: '#ef4444' },
        ]);
      },
      error: () => {
        this.kpis.set([
          { label: 'Total Productos', value: '—', delta: 'Sin conexion API', deltaPositive: false, icon: 'package', color: '#2563eb' },
          { label: 'Ordenes Fabricacion', value: '—', delta: 'Sin conexion API', deltaPositive: false, icon: 'factory', color: '#f59e0b' },
          { label: 'Ordenes Compra', value: '—', delta: 'Sin conexion API', deltaPositive: false, icon: 'cart', color: '#ef4444' },
          { label: 'Pedidos Venta', value: '—', delta: 'Sin conexion API', deltaPositive: false, icon: 'truck', color: '#10b981' },
          { label: 'Inspecciones', value: '—', delta: 'Sin conexion API', deltaPositive: false, icon: 'check', color: '#8b5cf6' },
          { label: 'Alertas Stock', value: '—', delta: 'Sin conexion API', deltaPositive: false, icon: 'alert', color: '#ef4444' },
        ]);
      },
    });
  }
}
