import { Component, ChangeDetectionStrategy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-movil-home',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="movil">
      <header class="movil__header">
        <div>
          <h1 class="movil__title">Planta</h1>
          <p class="movil__user">{{ userName() }}</p>
        </div>
      </header>

      <div class="movil__grid">
        <a class="tile tile--primary" routerLink="incidencia-nueva">
          <div class="tile__icon">⚠</div>
          <div class="tile__label">Reportar<br>Incidencia</div>
        </a>
        <a class="tile" routerLink="mis-ordenes">
          <div class="tile__icon">📋</div>
          <div class="tile__label">Mis Órdenes<br>de Trabajo</div>
        </a>
        <a class="tile" routerLink="incidencias">
          <div class="tile__icon">📑</div>
          <div class="tile__label">Incidencias<br>Abiertas</div>
        </a>
        <a class="tile" routerLink="activos">
          <div class="tile__icon">🏭</div>
          <div class="tile__label">Activos<br>y Máquinas</div>
        </a>
      </div>
    </div>
  `,
  styles: [`
    .movil { padding: 1rem; min-height: 100vh; background: #f5f5f7; }
    .movil__header { margin-bottom: 1.5rem; }
    .movil__title { font-size: 1.75rem; font-weight: 700; margin: 0; color: #111; }
    .movil__user { margin: 0.25rem 0 0; color: #666; font-size: 0.95rem; }
    .movil__grid { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; }
    .tile {
      display: flex; flex-direction: column; align-items: center; justify-content: center;
      padding: 1.5rem 1rem; background: white; border-radius: 16px;
      text-decoration: none; color: #111; box-shadow: 0 2px 8px rgba(0,0,0,0.06);
      min-height: 140px; text-align: center;
    }
    .tile--primary { background: #dc2626; color: white; }
    .tile__icon { font-size: 2.5rem; margin-bottom: 0.5rem; }
    .tile__label { font-size: 0.95rem; font-weight: 600; line-height: 1.3; }
  `],
})
export class MovilHomeComponent {
  private auth = inject(AuthService);
  userName = () => this.auth.currentUser()?.name ?? 'Operario';
}
