import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models/api.model';

export type TipoActivo = 'Maquina' | 'Linea' | 'Herramienta' | 'Vehiculo' | 'Instalacion' | 'Componente';
export type CriticidadActivo = 'Baja' | 'Media' | 'Alta' | 'Critica';
export type EstadoActivo = 'Operativo' | 'EnMantenimiento' | 'Averiado' | 'Parado' | 'Baja';

export interface ActivoListDto {
  id: string;
  codigo: string;
  nombre: string;
  tipo: TipoActivo;
  criticidad: CriticidadActivo;
  estado: EstadoActivo;
  ubicacion: string | null;
}

export interface ActivoDto {
  id: string;
  codigo: string;
  nombre: string;
  descripcion: string | null;
  tipo: TipoActivo;
  criticidad: CriticidadActivo;
  estado: EstadoActivo;
  activoPadreId: string | null;
  ubicacion: string | null;
  fabricante: string | null;
  modelo: string | null;
  numeroSerie: string | null;
  fechaAdquisicion: string | null;
  costeAdquisicion: number;
  horasUso: number;
  createdAt: string;
}

export interface CreateActivoRequest {
  codigo: string;
  nombre: string;
  tipo: TipoActivo;
  criticidad: CriticidadActivo;
  descripcion?: string;
  activoPadreId?: string;
  ubicacion?: string;
  fabricante?: string;
  modelo?: string;
  numeroSerie?: string;
  fechaAdquisicion?: string;
  costeAdquisicion?: number;
}

@Injectable({ providedIn: 'root' })
export class ActivosService {
  private api = inject(ApiService);

  listActivos(params: { search?: string; tipo?: TipoActivo; estado?: EstadoActivo; criticidad?: CriticidadActivo; page?: number; pageSize?: number } = {}): Observable<PagedResult<ActivoListDto>> {
    return this.api.get<PagedResult<ActivoListDto>>('/activos', {
      search: params.search,
      tipo: params.tipo,
      estado: params.estado,
      criticidad: params.criticidad,
      page: params.page ?? 1,
      pageSize: params.pageSize ?? 20,
    });
  }

  getActivo(id: string): Observable<ActivoDto> {
    return this.api.get<ActivoDto>(`/activos/${id}`);
  }

  createActivo(req: CreateActivoRequest): Observable<string> {
    return this.api.post<string>('/activos', req);
  }

  cambiarEstado(id: string, estado: EstadoActivo): Observable<string> {
    return this.api.put<string>(`/activos/${id}/estado`, { estado });
  }

  deleteActivo(id: string): Observable<string> {
    return this.api.delete<string>(`/activos/${id}`);
  }
}
