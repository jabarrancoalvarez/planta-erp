import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models/api.model';

export type ContextoIA = 'General' | 'Inventario' | 'Produccion' | 'Compras' | 'Ventas' | 'Calidad' | 'Mantenimiento' | 'RRHH' | 'Facturacion';
export type RolMensaje = 'User' | 'Assistant' | 'System';

export interface ConversacionListDto {
  id: string;
  titulo: string;
  contexto: ContextoIA;
  totalMensajes: number;
  totalTokensEntrada: number;
  totalTokensSalida: number;
  createdAt: string;
}

export interface MensajeIADto {
  id: string;
  rol: RolMensaje;
  contenido: string;
  tokensEntrada: number | null;
  tokensSalida: number | null;
  modelo: string | null;
  createdAt: string;
}

export interface ConversacionDetalleDto {
  id: string;
  titulo: string;
  contexto: ContextoIA;
  totalMensajes: number;
  totalTokensEntrada: number;
  totalTokensSalida: number;
  mensajes: MensajeIADto[];
}

export interface EnviarMensajeRequest {
  conversacionId?: string;
  mensaje: string;
  usuarioId: string;
  contexto: ContextoIA;
  titulo?: string;
}

export interface EnviarMensajeResult {
  conversacionId: string;
  respuesta: string;
  tokensEntrada: number;
  tokensSalida: number;
  modelo: string;
}

@Injectable({ providedIn: 'root' })
export class IaService {
  private api = inject(ApiService);

  enviar(req: EnviarMensajeRequest): Observable<EnviarMensajeResult> {
    return this.api.post<EnviarMensajeResult>('/ia/chat', req);
  }

  listConversaciones(usuarioId?: string): Observable<PagedResult<ConversacionListDto>> {
    return this.api.get<PagedResult<ConversacionListDto>>('/ia/conversaciones', {
      usuarioId,
      page: 1,
      pageSize: 20,
    });
  }

  getConversacion(id: string): Observable<ConversacionDetalleDto> {
    return this.api.get<ConversacionDetalleDto>(`/ia/conversaciones/${id}`);
  }
}
