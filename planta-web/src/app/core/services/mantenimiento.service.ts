import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models/api.model';

export type TipoMantenimiento = 'Preventivo' | 'Correctivo' | 'Predictivo' | 'Inspeccion';
export type EstadoOT = 'Borrador' | 'Planificada' | 'EnEjecucion' | 'EnEspera' | 'Completada' | 'Cancelada';
export type PrioridadOT = 'Baja' | 'Media' | 'Alta' | 'Urgente';
export type FrecuenciaPlan = 'Diaria' | 'Semanal' | 'Mensual' | 'Trimestral' | 'Semestral' | 'Anual' | 'PorHorasUso' | 'PorCiclos';

export interface OrdenTrabajoListDto {
  id: string;
  codigo: string;
  titulo: string;
  tipo: TipoMantenimiento;
  estado: EstadoOT;
  prioridad: PrioridadOT;
  activoId: string;
  fechaPlanificada: string | null;
}

export interface OrdenTrabajoDetailDto {
  id: string;
  codigo: string;
  titulo: string;
  descripcion: string | null;
  tipo: TipoMantenimiento;
  estado: EstadoOT;
  prioridad: PrioridadOT;
  activoId: string;
  asignadoAUserId: string | null;
  fechaPlanificada: string | null;
  fechaInicio: string | null;
  fechaFin: string | null;
  horasEstimadas: number;
  horasReales: number;
  costeManoObra: number;
  costeRepuestos: number;
  notasCierre: string | null;
}

export interface CreateOrdenTrabajoRequest {
  codigo: string;
  titulo: string;
  tipo: TipoMantenimiento;
  activoId: string;
  descripcion?: string;
  prioridad?: PrioridadOT;
  fechaPlanificada?: string;
  horasEstimadas?: number;
  planMantenimientoId?: string;
  incidenciaId?: string;
}

export interface CreatePlanRequest {
  codigo: string;
  nombre: string;
  activoId: string;
  frecuencia: FrecuenciaPlan;
  intervalo: number;
  descripcion?: string;
  horasEstimadas?: number;
  umbralHorasUso?: number;
  proximaEjecucion?: string;
}

export interface CompletarOTRequest {
  horasReales: number;
  costeManoObra: number;
  notasCierre?: string;
}

@Injectable({ providedIn: 'root' })
export class MantenimientoService {
  private api = inject(ApiService);

  listOrdenes(params: { estado?: EstadoOT; tipo?: TipoMantenimiento; activoId?: string; page?: number; pageSize?: number } = {}): Observable<PagedResult<OrdenTrabajoListDto>> {
    return this.api.get<PagedResult<OrdenTrabajoListDto>>('/mantenimiento/ordenes-trabajo', {
      estado: params.estado,
      tipo: params.tipo,
      activoId: params.activoId,
      page: params.page ?? 1,
      pageSize: params.pageSize ?? 20,
    });
  }

  createOrden(req: CreateOrdenTrabajoRequest): Observable<string> {
    return this.api.post<string>('/mantenimiento/ordenes-trabajo', req);
  }

  completarOrden(id: string, req: CompletarOTRequest): Observable<string> {
    return this.api.post<string>(`/mantenimiento/ordenes-trabajo/${id}/completar`, req);
  }

  getOrden(id: string): Observable<OrdenTrabajoDetailDto> {
    return this.api.get<OrdenTrabajoDetailDto>(`/mantenimiento/ordenes-trabajo/${id}`);
  }

  deleteOrden(id: string): Observable<string> {
    return this.api.delete<string>(`/mantenimiento/ordenes-trabajo/${id}`);
  }

  createPlan(req: CreatePlanRequest): Observable<string> {
    return this.api.post<string>('/mantenimiento/planes', req);
  }
}
