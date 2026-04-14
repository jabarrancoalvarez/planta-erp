import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models/api.model';

export type TipoImportacion = 'Productos' | 'Clientes' | 'Proveedores' | 'Empleados' | 'Activos' | 'Stock';
export type FormatoArchivo = 'Csv' | 'Xlsx';
export type EstadoImportJob = 'Pendiente' | 'Validando' | 'ListaParaImportar' | 'Importando' | 'Completada' | 'CompletadaConErrores' | 'Fallida' | 'Cancelada';

export interface ImportJobDto {
  id: string;
  tipo: TipoImportacion;
  formato: FormatoArchivo;
  estado: EstadoImportJob;
  nombreArchivo: string;
  filasTotales: number;
  filasValidas: number;
  filasConError: number;
  filasImportadas: number;
  resumenErrores: string | null;
  iniciadoEn: string | null;
  finalizadoEn: string | null;
}

export interface CreateImportJobRequest {
  tipo: TipoImportacion;
  formato: FormatoArchivo;
  nombreArchivo: string;
  userId: string;
}

@Injectable({ providedIn: 'root' })
export class ImportadorService {
  private api = inject(ApiService);

  listJobs(): Observable<PagedResult<ImportJobDto>> {
    return this.api.get<PagedResult<ImportJobDto>>('/importador/jobs', { page: 1, pageSize: 20 });
  }

  createJob(req: CreateImportJobRequest): Observable<string> {
    return this.api.post<string>('/importador/jobs', req);
  }
}
