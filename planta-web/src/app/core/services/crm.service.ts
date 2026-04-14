import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models/api.model';

export type EstadoLead = 'Nuevo' | 'Contactado' | 'Calificado' | 'Descartado' | 'Convertido';
export type OrigenLead = 'Web' | 'Recomendacion' | 'Evento' | 'LlamadaFria' | 'Marketing' | 'Otro';
export type FaseOportunidad = 'Prospecto' | 'Contactado' | 'PropuestaEnviada' | 'Negociacion' | 'Ganada' | 'Perdida';
export type TipoActividadCrm = 'Llamada' | 'Email' | 'Reunion' | 'Tarea' | 'Nota';

export interface LeadListDto {
  id: string;
  nombre: string;
  empresa: string | null;
  email: string | null;
  telefono: string | null;
  origen: OrigenLead;
  estado: EstadoLead;
  asignadoAUserId: string | null;
}

export interface OportunidadListDto {
  id: string;
  titulo: string;
  clienteId: string | null;
  fase: FaseOportunidad;
  importeEstimado: number;
  probabilidadPct: number;
  valorPonderado: number;
  fechaCierreEstimada: string | null;
}

export interface CreateLeadRequest {
  nombre: string;
  origen: OrigenLead;
  empresa?: string;
  email?: string;
  telefono?: string;
  notas?: string;
}

export interface CreateOportunidadRequest {
  titulo: string;
  importeEstimado: number;
  clienteId?: string;
  leadId?: string;
  fechaCierreEstimada?: string;
  descripcion?: string;
}

@Injectable({ providedIn: 'root' })
export class CrmService {
  private api = inject(ApiService);

  listLeads(params: { search?: string; estado?: EstadoLead; page?: number; pageSize?: number } = {}): Observable<PagedResult<LeadListDto>> {
    return this.api.get<PagedResult<LeadListDto>>('/crm/leads', {
      search: params.search,
      estado: params.estado,
      page: params.page ?? 1,
      pageSize: params.pageSize ?? 20,
    });
  }

  createLead(req: CreateLeadRequest): Observable<string> {
    return this.api.post<string>('/crm/leads', req);
  }

  listOportunidades(params: { fase?: FaseOportunidad; page?: number; pageSize?: number } = {}): Observable<PagedResult<OportunidadListDto>> {
    return this.api.get<PagedResult<OportunidadListDto>>('/crm/oportunidades', {
      fase: params.fase,
      page: params.page ?? 1,
      pageSize: params.pageSize ?? 20,
    });
  }

  createOportunidad(req: CreateOportunidadRequest): Observable<string> {
    return this.api.post<string>('/crm/oportunidades', req);
  }

  avanzarFase(id: string, nuevaFase: FaseOportunidad, probabilidadPct?: number): Observable<unknown> {
    return this.api.post<unknown>(`/crm/oportunidades/${id}/avanzar`, { nuevaFase, probabilidadPct });
  }
}
