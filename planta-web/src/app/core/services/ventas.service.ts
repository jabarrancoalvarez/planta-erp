import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models/api.model';

export interface ClienteListDto {
  id: string;
  razonSocial: string;
  nif: string;
  ciudad: string | null;
  email: string;
  activo: boolean;
}

export interface ClienteDetailDto {
  id: string;
  razonSocial: string;
  nif: string;
  email: string;
  direccionEnvio: string | null;
  direccionFacturacion: string | null;
  ciudad: string | null;
  codigoPostal: string | null;
  pais: string | null;
  telefono: string | null;
  web: string | null;
  activo: boolean;
  createdAt: string;
}

export interface PedidoListDto {
  id: string;
  codigo: string;
  clienteId: string;
  clienteRazonSocial: string;
  fechaEmision: string;
  estadoPedido: string;
  total: number;
}

export interface PedidoDetailDto {
  id: string;
  codigo: string;
  clienteId: string;
  clienteRazonSocial: string;
  fechaEmision: string;
  fechaEntregaEstimada: string | null;
  direccionEntrega: string | null;
  estadoPedido: string;
  observaciones: string | null;
  subtotal: number;
  impuestos: number;
  total: number;
  lineas: PedidoLineaDto[];
  createdAt: string;
}

export interface PedidoLineaDto {
  id: string;
  productoId: string;
  productoNombre: string;
  cantidad: number;
  precioUnitario: number;
  total: number;
}

export interface ExpedicionListDto {
  id: string;
  pedidoId: string;
  codigoPedido: string;
  fechaExpedicion: string;
  numeroSeguimiento: string | null;
  transportista: string | null;
  estadoExpedicion: string;
}

@Injectable({ providedIn: 'root' })
export class VentasService {
  private api = inject(ApiService);

  listClientes(search?: string, page = 1, pageSize = 20): Observable<PagedResult<ClienteListDto>> {
    return this.api.get('/ventas/clientes', { search, page, pageSize });
  }

  getCliente(id: string): Observable<ClienteDetailDto> {
    return this.api.get(`/ventas/clientes/${id}`);
  }

  createCliente(data: Partial<ClienteDetailDto>): Observable<string> {
    return this.api.post('/ventas/clientes', data);
  }

  updateCliente(id: string, data: Partial<ClienteDetailDto>): Observable<string> {
    return this.api.put(`/ventas/clientes/${id}`, data);
  }

  deleteCliente(id: string): Observable<string> {
    return this.api.delete(`/ventas/clientes/${id}`);
  }

  listPedidos(search?: string, estado?: string, page = 1, pageSize = 20): Observable<PagedResult<PedidoListDto>> {
    return this.api.get('/ventas/pedidos', { search, estado, page, pageSize });
  }

  getPedido(id: string): Observable<PedidoDetailDto> {
    return this.api.get(`/ventas/pedidos/${id}`);
  }

  createPedido(data: { clienteId: string; fechaEntregaEstimada?: string; direccionEntrega?: string; observaciones?: string }): Observable<string> {
    return this.api.post('/ventas/pedidos', data);
  }

  addLineaPedido(id: string, data: { productoId: string; cantidad: number; precioUnitario: number }): Observable<string> {
    return this.api.post(`/ventas/pedidos/${id}/lineas`, data);
  }

  cambiarEstadoPedido(id: string, estadoDestino: string, motivo?: string): Observable<string> {
    return this.api.put(`/ventas/pedidos/${id}/estado`, { estadoDestino, motivo });
  }

  deletePedido(id: string): Observable<string> {
    return this.api.delete(`/ventas/pedidos/${id}`);
  }

  listExpediciones(pedidoId?: string, estado?: string, page = 1, pageSize = 20): Observable<PagedResult<ExpedicionListDto>> {
    return this.api.get('/ventas/expediciones', { pedidoId, estado, page, pageSize });
  }
}
