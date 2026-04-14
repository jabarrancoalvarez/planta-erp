import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models/api.model';

export type TipoIncidencia = 'Averia' | 'Parada' | 'Seguridad' | 'CasiAccidente' | 'Calidad' | 'Otro';
export type SeveridadIncidencia = 'Baja' | 'Media' | 'Alta' | 'Critica';
export type EstadoIncidencia = 'Abierta' | 'EnRevision' | 'EnReparacion' | 'Resuelta' | 'Cerrada' | 'Descartada';

export interface IncidenciaListDto {
  id: string;
  codigo: string;
  titulo: string;
  tipo: TipoIncidencia;
  severidad: SeveridadIncidencia;
  estado: EstadoIncidencia;
  activoId: string | null;
  fechaApertura: string;
  paradaLinea: boolean;
}

export interface CreateIncidenciaRequest {
  codigo: string;
  titulo: string;
  descripcion: string;
  tipo: TipoIncidencia;
  severidad: SeveridadIncidencia;
  reportadoPorUserId: string;
  activoId?: string;
  ubicacionTexto?: string;
  paradaLinea?: boolean;
  fotosUrl?: string[];
}

export interface CerrarIncidenciaRequest {
  causaRaiz?: string;
  resolucionNotas?: string;
}

@Injectable({ providedIn: 'root' })
export class IncidenciasService {
  private api = inject(ApiService);

  listIncidencias(params: { estado?: EstadoIncidencia; severidad?: SeveridadIncidencia; tipo?: TipoIncidencia; activoId?: string; page?: number; pageSize?: number } = {}): Observable<PagedResult<IncidenciaListDto>> {
    return this.api.get<PagedResult<IncidenciaListDto>>('/incidencias', {
      estado: params.estado,
      severidad: params.severidad,
      tipo: params.tipo,
      activoId: params.activoId,
      page: params.page ?? 1,
      pageSize: params.pageSize ?? 20,
    });
  }

  createIncidencia(req: CreateIncidenciaRequest): Observable<string> {
    return this.api.post<string>('/incidencias', req);
  }

  cerrarIncidencia(id: string, req: CerrarIncidenciaRequest): Observable<string> {
    return this.api.post<string>(`/incidencias/${id}/cerrar`, req);
  }

  deleteIncidencia(id: string): Observable<string> {
    return this.api.delete<string>(`/incidencias/${id}`);
  }
}
