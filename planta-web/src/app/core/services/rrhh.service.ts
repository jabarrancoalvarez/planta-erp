import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models/api.model';

export type TipoFichaje = 'EntradaJornada' | 'SalidaJornada' | 'InicioDescanso' | 'FinDescanso' | 'InicioMaquina' | 'FinMaquina' | 'InicioOF' | 'FinOF';
export type TipoAusencia = 'Vacaciones' | 'BajaMedica' | 'AsuntoPropio' | 'Formacion' | 'Maternidad' | 'Paternidad' | 'Otro';
export type EstadoAusencia = 'Solicitada' | 'Aprobada' | 'Rechazada' | 'EnDisfrute' | 'Finalizada';

export interface EmpleadoListDto {
  id: string;
  codigo: string;
  nombre: string;
  apellidos: string;
  dni: string;
  puesto: string;
  departamento: string | null;
  activo: boolean;
}

export interface FichajeDto {
  id: string;
  empleadoId: string;
  empleadoNombre: string;
  tipo: TipoFichaje;
  momento: string;
  notas: string | null;
}

export interface AusenciaDto {
  id: string;
  empleadoId: string;
  empleadoNombre: string;
  tipo: TipoAusencia;
  estado: EstadoAusencia;
  fechaInicio: string;
  fechaFin: string;
  diasTotales: number;
  motivo: string | null;
}

export interface CreateEmpleadoRequest {
  codigo: string;
  nombre: string;
  apellidos: string;
  dni: string;
  puesto: string;
  costeHoraEstandar?: number;
  email?: string;
  telefono?: string;
  departamento?: string;
}

export interface RegistrarFichajeRequest {
  empleadoId: string;
  tipo: TipoFichaje;
  notas?: string;
}

export interface CreateAusenciaRequest {
  empleadoId: string;
  tipo: TipoAusencia;
  fechaInicio: string;
  fechaFin: string;
  motivo?: string;
}

@Injectable({ providedIn: 'root' })
export class RrhhService {
  private api = inject(ApiService);

  listEmpleados(params: { search?: string; page?: number; pageSize?: number } = {}): Observable<PagedResult<EmpleadoListDto>> {
    return this.api.get<PagedResult<EmpleadoListDto>>('/rrhh/empleados', {
      search: params.search,
      page: params.page ?? 1,
      pageSize: params.pageSize ?? 20,
    });
  }

  createEmpleado(req: CreateEmpleadoRequest): Observable<string> {
    return this.api.post<string>('/rrhh/empleados', req);
  }

  listFichajes(params: { empleadoId?: string; desde?: string; hasta?: string } = {}): Observable<PagedResult<FichajeDto>> {
    return this.api.get<PagedResult<FichajeDto>>('/rrhh/fichajes', {
      empleadoId: params.empleadoId,
      desde: params.desde,
      hasta: params.hasta,
      page: 1,
      pageSize: 50,
    });
  }

  registrarFichaje(req: RegistrarFichajeRequest): Observable<string> {
    return this.api.post<string>('/rrhh/fichajes', req);
  }

  listAusencias(params: { empleadoId?: string; estado?: EstadoAusencia } = {}): Observable<PagedResult<AusenciaDto>> {
    return this.api.get<PagedResult<AusenciaDto>>('/rrhh/ausencias', {
      empleadoId: params.empleadoId,
      estado: params.estado,
      page: 1,
      pageSize: 20,
    });
  }

  createAusencia(req: CreateAusenciaRequest): Observable<string> {
    return this.api.post<string>('/rrhh/ausencias', req);
  }

  aprobarAusencia(id: string, userId: string): Observable<unknown> {
    return this.api.post<unknown>(`/rrhh/ausencias/${id}/aprobar`, { userId });
  }

  rechazarAusencia(id: string, userId: string): Observable<unknown> {
    return this.api.post<unknown>(`/rrhh/ausencias/${id}/rechazar`, { userId });
  }
}
