import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models/api.model';

export type EstadoFactura = 'Borrador' | 'Emitida' | 'Firmada' | 'EnviadaVerifactu' | 'Aceptada' | 'Rechazada' | 'Anulada';
export type EstadoVerifactu = 'NoEnviada' | 'Pendiente' | 'Enviada' | 'Aceptada' | 'RechazadaPorAEAT' | 'Error';
export type TipoFactura = 'Ordinaria' | 'Rectificativa' | 'Simplificada' | 'Abono';

export interface FacturaListDto {
  id: string;
  numeroCompleto: string;
  clienteId: string;
  clienteNombre: string;
  fechaEmision: string;
  total: number;
  estado: EstadoFactura;
  estadoVerifactu: EstadoVerifactu;
}

export interface LineaFacturaDto {
  id: string;
  numeroLinea: number;
  descripcion: string;
  cantidad: number;
  precioUnitario: number;
  descuentoPct: number;
  ivaPct: number;
  baseImponible: number;
  iva: number;
  total: number;
  productoId: string | null;
}

export interface FacturaDto {
  id: string;
  numeroCompleto: string;
  serieCodigo: string;
  numero: number;
  ejercicio: number;
  tipo: TipoFactura;
  estado: EstadoFactura;
  clienteId: string;
  clienteNombre: string;
  clienteNIF: string | null;
  clienteDireccion: string | null;
  fechaEmision: string;
  fechaVencimiento: string | null;
  baseImponible: number;
  totalIva: number;
  total: number;
  observaciones: string | null;
  estadoVerifactu: EstadoVerifactu;
  verifactuCsv: string | null;
  codigoQrVerifactu: string | null;
  lineas: LineaFacturaDto[];
}

export interface LineaFacturaInput {
  descripcion: string;
  cantidad: number;
  precioUnitario: number;
  ivaPct: number;
  descuentoPct?: number;
  productoId?: string;
}

export interface CreateFacturaRequest {
  serieCodigo: string;
  clienteId: string;
  clienteNombre: string;
  lineas: LineaFacturaInput[];
  tipo?: TipoFactura;
  clienteNIF?: string;
  clienteDireccion?: string;
  fechaEmision?: string;
  fechaVencimiento?: string;
  observaciones?: string;
}

@Injectable({ providedIn: 'root' })
export class FacturacionService {
  private api = inject(ApiService);

  listFacturas(params: { estado?: EstadoFactura; clienteId?: string; page?: number; pageSize?: number } = {}): Observable<PagedResult<FacturaListDto>> {
    return this.api.get<PagedResult<FacturaListDto>>('/facturacion/facturas', {
      estado: params.estado,
      clienteId: params.clienteId,
      page: params.page ?? 1,
      pageSize: params.pageSize ?? 20,
    });
  }

  getFactura(id: string): Observable<FacturaDto> {
    return this.api.get<FacturaDto>(`/facturacion/facturas/${id}`);
  }

  createFactura(req: CreateFacturaRequest): Observable<string> {
    return this.api.post<string>('/facturacion/facturas', req);
  }

  emitir(id: string): Observable<unknown> {
    return this.api.post<unknown>(`/facturacion/facturas/${id}/emitir`, {});
  }

  enviarVerifactu(id: string): Observable<unknown> {
    return this.api.post<unknown>(`/facturacion/facturas/${id}/verifactu`, {});
  }

  deleteFactura(id: string): Observable<unknown> {
    return this.api.delete(`/facturacion/facturas/${id}`);
  }

  descargarPdf(id: string): Observable<Blob> {
    return this.api.getBlob(`/facturacion/facturas/${id}/pdf`);
  }
}
