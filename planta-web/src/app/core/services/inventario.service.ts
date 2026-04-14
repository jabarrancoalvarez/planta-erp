import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models/api.model';

export interface ProductoListDto {
  id: string;
  sku: string;
  nombre: string;
  tipo: string;
  unidadMedida: string;
  precioVenta: number;
  activo: boolean;
}

export interface ProductoDetailDto {
  id: string;
  sku: string;
  nombre: string;
  descripcion: string | null;
  tipo: string;
  unidadMedida: string;
  categoriaId: string | null;
  precioCompra: number;
  precioVenta: number;
  pesoKg: number;
  codigoBarras: string | null;
  imagenUrl: string | null;
  activo: boolean;
  createdAt: string;
}

export interface AlmacenListDto {
  id: string;
  nombre: string;
  direccion: string | null;
  esPrincipal: boolean;
  totalUbicaciones: number;
}

export interface UbicacionDto {
  id: string;
  codigo: string;
  nombre: string | null;
  capacidadMaxima: number;
  activa: boolean;
}

export interface AlmacenDetailDto {
  id: string;
  nombre: string;
  direccion: string | null;
  descripcion: string | null;
  esPrincipal: boolean;
  ubicaciones: UbicacionDto[];
}

export interface MovimientoStockDto {
  id: string;
  productoId: string;
  almacenId: string;
  ubicacionId: string | null;
  loteId: string | null;
  tipo: string;
  cantidad: number;
  cantidadAnterior: number;
  cantidadPosterior: number;
  referencia: string | null;
  notas: string | null;
  createdAt: string;
}

export interface LoteListDto {
  id: string;
  codigo: string;
  cantidadActual: number;
  estado: string;
  fechaCaducidad: string | null;
}

export interface AlertaStockDto {
  id: string;
  productoId: string;
  almacenId: string | null;
  stockMinimo: number;
  stockMaximo: number;
  puntoReorden: number;
  cantidadReorden: number;
  autoReorden: boolean;
  activa: boolean;
}

@Injectable({ providedIn: 'root' })
export class InventarioService {
  private api = inject(ApiService);

  listProductos(search?: string, page = 1, pageSize = 20): Observable<PagedResult<ProductoListDto>> {
    return this.api.get('/inventario/productos', { search, page, pageSize });
  }

  getProducto(id: string): Observable<ProductoDetailDto> {
    return this.api.get(`/inventario/productos/${id}`);
  }

  createProducto(data: Partial<ProductoDetailDto>): Observable<string> {
    return this.api.post('/inventario/productos', data);
  }

  updateProducto(id: string, data: Partial<ProductoDetailDto>): Observable<string> {
    return this.api.put(`/inventario/productos/${id}`, data);
  }

  deleteProducto(id: string): Observable<string> {
    return this.api.delete(`/inventario/productos/${id}`);
  }

  listAlmacenes(): Observable<AlmacenListDto[]> {
    return this.api.get('/inventario/almacenes');
  }

  getAlmacen(id: string): Observable<AlmacenDetailDto> {
    return this.api.get(`/inventario/almacenes/${id}`);
  }

  deleteAlmacen(id: string): Observable<string> {
    return this.api.delete(`/inventario/almacenes/${id}`);
  }

  listMovimientos(productoId?: string, almacenId?: string, page = 1, pageSize = 20): Observable<PagedResult<MovimientoStockDto>> {
    return this.api.get('/inventario/movimientos', { productoId, almacenId, page, pageSize });
  }

  listLotes(productoId?: string, page = 1, pageSize = 20): Observable<PagedResult<LoteListDto>> {
    return this.api.get('/inventario/lotes', { productoId, page, pageSize });
  }

  listAlertas(productoId?: string): Observable<AlertaStockDto[]> {
    return this.api.get('/inventario/alertas', { productoId });
  }
}
