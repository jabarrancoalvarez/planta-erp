import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, map } from 'rxjs/operators';
import { NavigationEnd } from '@angular/router';

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="wrap">
      <div class="card">
        <div class="icon">{{ isForbidden() ? '🔒' : '🧭' }}</div>
        <h1>{{ isForbidden() ? 'Acceso restringido' : 'Página no encontrada' }}</h1>
        <p>
          {{
            isForbidden()
              ? 'Tu rol actual no tiene permiso para acceder a esta sección. Si crees que es un error, contacta con el administrador.'
              : 'La ruta que intentas abrir no existe o fue movida.'
          }}
        </p>
        <div class="actions">
          <a routerLink="/app/dashboard" class="primary">Ir al dashboard</a>
        </div>
      </div>
    </div>
  `,
  styles: [
    `
      .wrap {
        display: flex;
        align-items: center;
        justify-content: center;
        min-height: 60vh;
        padding: 2rem;
      }
      .card {
        max-width: 420px;
        text-align: center;
        background: var(--color-surface, #fff);
        padding: 2.5rem 2rem;
        border-radius: 16px;
        box-shadow: 0 4px 24px rgba(0, 0, 0, 0.08);
      }
      .icon {
        font-size: 3rem;
        margin-bottom: 0.5rem;
      }
      h1 {
        font-size: 1.5rem;
        margin: 0 0 0.75rem;
        color: var(--color-text, #1f2937);
      }
      p {
        color: var(--color-text-muted, #6b7280);
        margin: 0 0 1.5rem;
        line-height: 1.5;
      }
      .actions {
        display: flex;
        justify-content: center;
      }
      .primary {
        background: var(--color-primary, #f97316);
        color: #fff;
        padding: 0.65rem 1.4rem;
        border-radius: 8px;
        text-decoration: none;
        font-weight: 600;
      }
      .primary:hover {
        opacity: 0.9;
      }
    `,
  ],
})
export class NotFoundComponent {
  private router = inject(Router);

  private currentUrl = toSignal(
    this.router.events.pipe(
      filter(e => e instanceof NavigationEnd),
      map(() => this.router.url)
    ),
    { initialValue: this.router.url }
  );

  readonly isForbidden = computed(() => (this.currentUrl() ?? '').includes('/forbidden'));
}
