import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models/api.model';

export interface BOMListDto {
  id: string;
  productoId: string;
  nombre: string;
  versionBOM: number;
  activo: boolean;
  numeroLineas: number;
}

export interface BOMDetailDto {
  id: string;
  productoId: string;
  nombre: string;
  descripcion: string | null;
  versionBOM: number;
  activo: boolean;
  lineas: BOMLineaDto[];
  createdAt: string;
}

export interface BOMLineaDto {
  id: string;
  componenteId: string;
  componenteNombre: string;
  cantidad: number;
  unidadMedida: string;
  mermaEstimada: number;
}

export interface RutaListDto {
  id: string;
  productoId: string;
  nombre: string;
  activa: boolean;
  numeroOperaciones: number;
}

export interface OperacionRutaDto {
  id: string;
  numero: number;
  nombre: string;
  tipoOperacion: string;
  tiempoEstimadoMinutos: number;
  centroTrabajo: string;
  instrucciones: string | null;
}

export interface RutaDetailDto {
  id: string;
  productoId: string;
  nombre: string;
  descripcion: string | null;
  activa: boolean;
  createdAt: string;
  operaciones: OperacionRutaDto[];
}

export interface OFListDto {
  id: string;
  codigoOF: string;
  productoId: string;
  cantidadPlanificada: number;
  unidadMedida: string;
  estadoOF: string;
  fechaInicio: string | null;
  prioridad: number;
}

export interface OFDetailDto {
  id: string;
  codigoOF: string;
  productoId: string;
  listaMaterialesId: string | null;
  cantidadPlanificada: number;
  cantidadProducida: number;
  unidadMedida: string;
  estadoOF: string;
  prioridad: number;
  fechaInicio: string | null;
  fechaFinEstimada: string | null;
  fechaFinReal: string | null;
  observaciones: string | null;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class ProduccionService {
  private api = inject(ApiService);

  listBOMs(search?: string, page = 1, pageSize = 20): Observable<PagedResult<BOMListDto>> {
    return this.api.get('/produccion/bom', { search, page, pageSize });
  }

  getBOM(id: string): Observable<BOMDetailDto> {
    return this.api.get(`/produccion/bom/${id}`);
  }

  createBOM(data: { nombre: string; productoId: string; descripcion?: string }): Observable<string> {
    return this.api.post('/produccion/bom', data);
  }

  deleteBOM(id: string): Observable<string> {
    return this.api.delete(`/produccion/bom/${id}`);
  }

  listRutas(search?: string, page = 1, pageSize = 20): Observable<PagedResult<RutaListDto>> {
    return this.api.get('/produccion/rutas', { search, page, pageSize });
  }

  getRuta(id: string): Observable<RutaDetailDto> {
    return this.api.get(`/produccion/rutas/${id}`);
  }

  deleteRuta(id: string): Observable<string> {
    return this.api.delete(`/produccion/rutas/${id}`);
  }

  listOFs(search?: string, estado?: string, page = 1, pageSize = 20): Observable<PagedResult<OFListDto>> {
    return this.api.get('/produccion/ordenes', { search, estado, page, pageSize });
  }

  getOF(id: string): Observable<OFDetailDto> {
    return this.api.get(`/produccion/ordenes/${id}`);
  }

  createOF(data: {
    productoId: string;
    listaMaterialesId?: string;
    cantidadPlanificada: number;
    unidadMedida: string;
    prioridad: number;
    observaciones?: string;
  }): Observable<string> {
    return this.api.post('/produccion/ordenes', data);
  }

  cambiarEstadoOF(id: string, estadoDestino: string, motivo?: string): Observable<string> {
    return this.api.put(`/produccion/ordenes/${id}/estado`, { estadoDestino, motivo });
  }

  deleteOF(id: string): Observable<string> {
    return this.api.delete(`/produccion/ordenes/${id}`);
  }

  registrarProduccion(id: string, data: { cantidadProducida: number; observaciones?: string }): Observable<string> {
    return this.api.post(`/produccion/ordenes/${id}/produccion`, data);
  }
}
