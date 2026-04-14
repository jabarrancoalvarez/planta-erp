import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models/api.model';

export type TipoCoste = 'Material' | 'ManoObra' | 'Maquina' | 'Subcontratacion' | 'Indirecto' | 'Otro';
export type OrigenImputacion = 'Manual' | 'FichajeOperario' | 'ConsumoStock' | 'ParteProduccion' | 'OrdenTrabajo';

export interface ImputacionCosteDto {
  id: string;
  ordenFabricacionId: string | null;
  ordenTrabajoId: string | null;
  productoId: string | null;
  tipo: TipoCoste;
  origen: OrigenImputacion;
  cantidad: number;
  precioUnitario: number;
  importe: number;
  concepto: string | null;
  fecha: string;
}

export interface CreateImputacionRequest {
  tipo: TipoCoste;
  origen: OrigenImputacion;
  cantidad: number;
  precioUnitario: number;
  ordenFabricacionId?: string;
  ordenTrabajoId?: string;
  productoId?: string;
  concepto?: string;
  fecha?: string;
}

@Injectable({ providedIn: 'root' })
export class CostesService {
  private api = inject(ApiService);

  listImputaciones(params: { ordenFabricacionId?: string; ordenTrabajoId?: string } = {}): Observable<PagedResult<ImputacionCosteDto>> {
    return this.api.get<PagedResult<ImputacionCosteDto>>('/costes/imputaciones', {
      ordenFabricacionId: params.ordenFabricacionId,
      ordenTrabajoId: params.ordenTrabajoId,
      page: 1,
      pageSize: 50,
    });
  }

  createImputacion(req: CreateImputacionRequest): Observable<string> {
    return this.api.post<string>('/costes/imputaciones', req);
  }

  updateImputacion(id: string, data: { cantidad: number; precioUnitario: number; concepto: string | null; fecha: string }): Observable<unknown> {
    return this.api.put(`/costes/imputaciones/${id}`, data);
  }

  deleteImputacion(id: string): Observable<unknown> {
    return this.api.delete(`/costes/imputaciones/${id}`);
  }
}
