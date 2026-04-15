import { Injectable, inject, signal } from '@angular/core';
import { ApiService } from './api.service';

export type TipoFichaje = 'EntradaJornada' | 'SalidaJornada' | 'InicioDescanso' | 'FinDescanso';

interface QueuedFichaje {
  id: string;
  tipo: TipoFichaje;
  timestamp: string;
  notas?: string;
}

const STORAGE_KEY = 'planta_fichajes_pendientes';

@Injectable({ providedIn: 'root' })
export class FichajesOfflineService {
  private api = inject(ApiService);

  readonly pendientes = signal<QueuedFichaje[]>(this.load());
  readonly online = signal<boolean>(navigator.onLine);

  constructor() {
    window.addEventListener('online', () => { this.online.set(true); this.flush(); });
    window.addEventListener('offline', () => this.online.set(false));
    if (this.online() && this.pendientes().length > 0) setTimeout(() => this.flush(), 1000);
  }

  registrar(tipo: TipoFichaje, notas?: string): Promise<{ ok: boolean; queued: boolean }> {
    const payload = { tipo, notas };
    if (!this.online()) {
      this.enqueue(tipo, notas);
      return Promise.resolve({ ok: true, queued: true });
    }
    return new Promise((resolve) => {
      this.api.post('/rrhh/fichajes/me', payload).subscribe({
        next: () => resolve({ ok: true, queued: false }),
        error: () => {
          this.enqueue(tipo, notas);
          resolve({ ok: true, queued: true });
        },
      });
    });
  }

  async flush(): Promise<void> {
    const queue = [...this.pendientes()];
    if (queue.length === 0) return;
    const remaining: QueuedFichaje[] = [];
    for (const item of queue) {
      try {
        await new Promise<void>((resolve, reject) => {
          this.api.post('/rrhh/fichajes/me', { tipo: item.tipo, notas: item.notas }).subscribe({
            next: () => resolve(),
            error: (err) => reject(err),
          });
        });
      } catch {
        remaining.push(item);
      }
    }
    this.save(remaining);
  }

  private enqueue(tipo: TipoFichaje, notas?: string): void {
    const item: QueuedFichaje = { id: crypto.randomUUID(), tipo, notas, timestamp: new Date().toISOString() };
    const next = [...this.pendientes(), item];
    this.save(next);
  }

  private load(): QueuedFichaje[] {
    try {
      return JSON.parse(localStorage.getItem(STORAGE_KEY) ?? '[]');
    } catch { return []; }
  }

  private save(items: QueuedFichaje[]): void {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(items));
    this.pendientes.set(items);
  }
}
