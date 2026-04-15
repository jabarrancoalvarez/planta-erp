import { Component, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FichajesOfflineService, TipoFichaje } from '../../core/services/fichajes-offline.service';

@Component({
  selector: 'app-fichar',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="fichar">
      <header class="fichar__header">
        <a routerLink="/app/movil" class="fichar__back">← Atrás</a>
        <div class="fichar__status" [class.fichar__status--offline]="!offline.online()">
          {{ offline.online() ? '● Online' : '● Offline' }}
        </div>
      </header>

      <h1 class="fichar__title">Fichar jornada</h1>
      <p class="fichar__now">{{ now() }}</p>

      @if (offline.pendientes().length > 0) {
        <div class="fichar__pending">
          {{ offline.pendientes().length }} fichaje(s) pendiente(s) de sincronizar
          @if (offline.online()) {
            <button class="fichar__sync" (click)="offline.flush()">Sincronizar ahora</button>
          }
        </div>
      }

      <div class="fichar__buttons">
        <button class="fichar__btn fichar__btn--entrada" [disabled]="loading()" (click)="registrar('EntradaJornada')">
          <span class="fichar__btn-icon">▶</span>
          <span>Entrada</span>
        </button>
        <button class="fichar__btn fichar__btn--pausa" [disabled]="loading()" (click)="registrar('InicioDescanso')">
          <span class="fichar__btn-icon">❚❚</span>
          <span>Inicio pausa</span>
        </button>
        <button class="fichar__btn fichar__btn--pausa" [disabled]="loading()" (click)="registrar('FinDescanso')">
          <span class="fichar__btn-icon">▶</span>
          <span>Fin pausa</span>
        </button>
        <button class="fichar__btn fichar__btn--salida" [disabled]="loading()" (click)="registrar('SalidaJornada')">
          <span class="fichar__btn-icon">■</span>
          <span>Salida</span>
        </button>
      </div>

      @if (mensaje()) {
        <div class="fichar__toast" [class.fichar__toast--queued]="queued()">{{ mensaje() }}</div>
      }
    </div>
  `,
  styles: [`
    .fichar { padding: 1rem; min-height: 100vh; background: #f5f5f7; display: flex; flex-direction: column; }
    .fichar__header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem; }
    .fichar__back { color: #dc2626; text-decoration: none; font-weight: 600; }
    .fichar__status { font-size: 0.85rem; color: #10b981; font-weight: 600; }
    .fichar__status--offline { color: #ef4444; }
    .fichar__title { font-size: 1.75rem; font-weight: 700; margin: 0 0 0.25rem; color: #111; }
    .fichar__now { margin: 0 0 1.5rem; color: #666; font-size: 1rem; }
    .fichar__pending { padding: 0.75rem 1rem; background: #fef3c7; color: #92400e; border-radius: 10px; margin-bottom: 1rem; font-size: 0.9rem; display: flex; justify-content: space-between; align-items: center; gap: 0.5rem; }
    .fichar__sync { background: #dc2626; color: white; border: 0; padding: 0.5rem 0.85rem; border-radius: 8px; font-weight: 600; }
    .fichar__buttons { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; }
    .fichar__btn { display: flex; flex-direction: column; align-items: center; justify-content: center; min-height: 140px; border: 0; border-radius: 16px; color: white; font-size: 1.1rem; font-weight: 600; cursor: pointer; box-shadow: 0 4px 12px rgba(0,0,0,0.12); }
    .fichar__btn[disabled] { opacity: 0.5; }
    .fichar__btn-icon { font-size: 2rem; margin-bottom: 0.5rem; }
    .fichar__btn--entrada { background: #10b981; }
    .fichar__btn--salida { background: #ef4444; }
    .fichar__btn--pausa { background: #f59e0b; }
    .fichar__toast { margin-top: 1.25rem; padding: 0.85rem 1rem; background: #10b981; color: white; border-radius: 10px; text-align: center; font-weight: 600; }
    .fichar__toast--queued { background: #f59e0b; }
  `],
})
export class FicharComponent {
  readonly offline = inject(FichajesOfflineService);
  readonly loading = signal(false);
  readonly mensaje = signal<string | null>(null);
  readonly queued = signal(false);
  readonly now = signal(new Date().toLocaleString('es-ES'));

  constructor() { setInterval(() => this.now.set(new Date().toLocaleString('es-ES')), 1000); }

  async registrar(tipo: TipoFichaje): Promise<void> {
    this.loading.set(true);
    this.mensaje.set(null);
    const res = await this.offline.registrar(tipo);
    this.loading.set(false);
    this.queued.set(res.queued);
    this.mensaje.set(res.queued ? `${tipo} guardado offline — se sincronizará automáticamente` : `${tipo} registrado`);
    setTimeout(() => this.mensaje.set(null), 3000);
  }
}
