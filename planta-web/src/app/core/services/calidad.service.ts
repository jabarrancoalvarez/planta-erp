import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models/api.model';

export interface PlantillaListDto {
  id: string;
  nombre: string;
  origenInspeccion: string;
  version: number;
  activa: boolean;
  cantidadCriterios: number;
}

export interface PlantillaDetailDto {
  id: string;
  nombre: string;
  descripcion: string | null;
  origenInspeccion: string;
  version: number;
  activa: boolean;
  criterios: CriterioDto[];
  createdAt: string;
}

export interface CriterioDto {
  id: string;
  nombre: string;
  descripcion: string | null;
  valorMinimo: number | null;
  valorMaximo: number | null;
  unidadMedida: string | null;
  obligatorio: boolean;
}

export interface InspeccionListDto {
  id: string;
  origenInspeccion: string;
  referenciaOrigenId: string;
  fechaInspeccion: string;
  resultadoInspeccion: string | null;
}

export interface NCListDto {
  id: string;
  codigo: string;
  origenInspeccion: string;
  severidadNC: string;
  estadoNoConformidad: string;
  fechaDeteccion: string;
}

export interface NCDetailDto {
  id: string;
  codigo: string;
  descripcion: string | null;
  origenInspeccion: string;
  severidadNC: string;
  estadoNoConformidad: string;
  causaRaiz: string | null;
  fechaDeteccion: string;
  accionesCorrectivas: AccionCorrectivaDto[];
  createdAt: string;
}

export interface AccionCorrectivaDto {
  id: string;
  descripcion: string;
  responsable: string | null;
  fechaLimite: string | null;
  fechaCierre: string | null;
  estado: string;
}

@Injectable({ providedIn: 'root' })
export class CalidadService {
  private api = inject(ApiService);

  listPlantillas(search?: string, page = 1, pageSize = 20): Observable<PagedResult<PlantillaListDto>> {
    return this.api.get('/calidad/plantillas', { search, page, pageSize });
  }

  getPlantilla(id: string): Observable<PlantillaDetailDto> {
    return this.api.get(`/calidad/plantillas/${id}`);
  }

  createPlantilla(data: { nombre: string; descripcion?: string; origenInspeccion: string }): Observable<string> {
    return this.api.post('/calidad/plantillas', data);
  }

  addCriterio(plantillaId: string, data: {
    nombre: string;
    descripcion?: string;
    valorMinimo?: number;
    valorMaximo?: number;
    unidadMedida?: string;
    obligatorio: boolean;
  }): Observable<string> {
    return this.api.post(`/calidad/plantillas/${plantillaId}/criterios`, data);
  }

  listInspecciones(origen?: string, resultado?: string, page = 1, pageSize = 20): Observable<PagedResult<InspeccionListDto>> {
    return this.api.get('/calidad/inspecciones', { origen, resultado, page, pageSize });
  }

  listNCs(estado?: string, severidad?: string, page = 1, pageSize = 20): Observable<PagedResult<NCListDto>> {
    return this.api.get('/calidad/no-conformidades', { estado, severidad, page, pageSize });
  }

  getNC(id: string): Observable<NCDetailDto> {
    return this.api.get(`/calidad/no-conformidades/${id}`);
  }

  addAccionCorrectiva(ncId: string, data: {
    descripcion: string;
    responsable?: string;
    fechaLimite?: string;
  }): Observable<string> {
    return this.api.post(`/calidad/no-conformidades/${ncId}/acciones`, data);
  }

  cambiarEstadoNC(id: string, estadoDestino: string, causaRaiz?: string): Observable<string> {
    return this.api.put(`/calidad/no-conformidades/${id}/estado`, { estadoDestino, causaRaiz });
  }
}
