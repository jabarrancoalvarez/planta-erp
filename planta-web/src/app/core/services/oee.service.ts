import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models/api.model';

export interface RegistroOEEDto {
  id: string;
  activoId: string;
  turnoId: string | null;
  ordenFabricacionId: string | null;
  fecha: string;
  minutosPlanificados: number;
  minutosFuncionamiento: number;
  piezasTotales: number;
  piezasBuenas: number;
  tiempoCicloTeoricoSeg: number;
  disponibilidad: number;
  rendimiento: number;
  calidad: number;
  oee: number;
  notas: string | null;
}

export interface RegistrarOEERequest {
  activoId: string;
  fecha: string;
  minutosPlanificados: number;
  minutosFuncionamiento: number;
  piezasTotales: number;
  piezasBuenas: number;
  tiempoCicloTeoricoSeg: number;
  notas?: string;
}

@Injectable({ providedIn: 'root' })
export class OeeService {
  private api = inject(ApiService);

  listRegistros(params: { activoId?: string } = {}): Observable<PagedResult<RegistroOEEDto>> {
    return this.api.get<PagedResult<RegistroOEEDto>>('/oee/registros', {
      activoId: params.activoId,
      page: 1,
      pageSize: 50,
    });
  }

  registrar(req: RegistrarOEERequest): Observable<string> {
    return this.api.post<string>('/oee/registros', req);
  }
}
