import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models/api.model';

export interface ProveedorListDto {
  id: string;
  razonSocial: string;
  nif: string;
  ciudad: string | null;
  email: string;
  activo: boolean;
}

export interface ProveedorDetailDto {
  id: string;
  razonSocial: string;
  nif: string;
  email: string;
  direccion: string | null;
  ciudad: string | null;
  codigoPostal: string | null;
  pais: string | null;
  telefono: string | null;
  web: string | null;
  diasVencimiento: number;
  descuentoProntoPago: number;
  metodoPago: string | null;
  activo: boolean;
  createdAt: string;
}

export interface OCListDto {
  id: string;
  codigo: string;
  proveedorId: string;
  proveedorRazonSocial: string;
  fechaEmision: string;
  estadoOC: string;
  total: number;
}

export interface OCDetailDto {
  id: string;
  codigo: string;
  proveedorId: string;
  proveedorRazonSocial: string;
  fechaEmision: string;
  fechaEntregaEstimada: string | null;
  estadoOC: string;
  observaciones: string | null;
  subtotal: number;
  impuestos: number;
  total: number;
  lineas: OCLineaDto[];
  createdAt: string;
}

export interface OCLineaDto {
  id: string;
  productoId: string;
  productoNombre: string;
  cantidad: number;
  precioUnitario: number;
  total: number;
}

export interface RecepcionListDto {
  id: string;
  ordenCompraId: string;
  codigoOC: string;
  fechaRecepcion: string;
  numeroAlbaran: string | null;
  estadoRecepcion: string;
}

@Injectable({ providedIn: 'root' })
export class ComprasService {
  private api = inject(ApiService);

  listProveedores(search?: string, page = 1, pageSize = 20): Observable<PagedResult<ProveedorListDto>> {
    return this.api.get('/compras/proveedores', { search, page, pageSize });
  }

  getProveedor(id: string): Observable<ProveedorDetailDto> {
    return this.api.get(`/compras/proveedores/${id}`);
  }

  createProveedor(data: Partial<ProveedorDetailDto>): Observable<string> {
    return this.api.post('/compras/proveedores', data);
  }

  updateProveedor(id: string, data: Partial<ProveedorDetailDto>): Observable<string> {
    return this.api.put(`/compras/proveedores/${id}`, data);
  }

  deleteProveedor(id: string): Observable<string> {
    return this.api.delete(`/compras/proveedores/${id}`);
  }

  listOCs(search?: string, estado?: string, page = 1, pageSize = 20): Observable<PagedResult<OCListDto>> {
    return this.api.get('/compras/ordenes', { search, estado, page, pageSize });
  }

  getOC(id: string): Observable<OCDetailDto> {
    return this.api.get(`/compras/ordenes/${id}`);
  }

  createOC(data: { proveedorId: string; fechaEntregaEstimada?: string; observaciones?: string }): Observable<string> {
    return this.api.post('/compras/ordenes', data);
  }

  addLineaOC(id: string, data: { productoId: string; cantidad: number; precioUnitario: number }): Observable<string> {
    return this.api.post(`/compras/ordenes/${id}/lineas`, data);
  }

  cambiarEstadoOC(id: string, estadoDestino: string, motivo?: string): Observable<string> {
    return this.api.put(`/compras/ordenes/${id}/estado`, { estadoDestino, motivo });
  }

  deleteOC(id: string): Observable<string> {
    return this.api.delete(`/compras/ordenes/${id}`);
  }

  listRecepciones(ordenCompraId?: string, estado?: string, page = 1, pageSize = 20): Observable<PagedResult<RecepcionListDto>> {
    return this.api.get('/compras/recepciones', { ordenCompraId, estado, page, pageSize });
  }
}
