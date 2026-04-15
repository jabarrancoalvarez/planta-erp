import { Injectable, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { ApiService } from './api.service';

export interface NotificacionDto {
  id: string;
  titulo: string;
  mensaje: string;
  tipo: string;
  url: string | null;
  leida: boolean;
  createdAt: string;
  usuarioId: string | null;
}

export interface NotificacionesPage {
  items: NotificacionDto[];
  noLeidas: number;
}

@Injectable({ providedIn: 'root' })
export class NotificacionesService {
  private api = inject(ApiService);

  readonly items = signal<NotificacionDto[]>([]);
  readonly noLeidas = signal(0);

  list(soloNoLeidas = false, take = 50): Observable<NotificacionesPage> {
    return this.api.get<NotificacionesPage>('/notificaciones', { soloNoLeidas, take }).pipe(
      tap(r => { this.items.set(r.items ?? []); this.noLeidas.set(r.noLeidas ?? 0); })
    );
  }

  marcarLeida(id: string): Observable<void> {
    return this.api.post<void>(`/notificaciones/${id}/marcar-leida`, {});
  }

  marcarTodas(): Observable<{ marcadas: number }> {
    return this.api.post<{ marcadas: number }>(`/notificaciones/marcar-todas-leidas`, {});
  }

  crear(req: { titulo: string; mensaje: string; tipo?: string; usuarioId?: string; url?: string }): Observable<{ id: string }> {
    return this.api.post<{ id: string }>('/notificaciones', req);
  }
}
