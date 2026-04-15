import { Component, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-onboarding',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="onboarding">
      <div class="onboarding__card">
        <h1 class="onboarding__title">¡Bienvenido a PlanTA, {{ userName() }}!</h1>
        <p class="onboarding__subtitle">
          Tu empresa <strong>{{ empresa() }}</strong> está lista. Tienes 14 días de trial para explorar.
        </p>
        <p class="onboarding__lead">¿Cómo quieres empezar?</p>

        <div class="onboarding__options">
          <button class="option-card" [disabled]="loading()" (click)="empezarVacio()">
            <div class="option-card__icon">📋</div>
            <h3>Empezar vacío</h3>
            <p>Configurar todo desde cero. Ideal si ya tienes clara la estructura de tu negocio.</p>
          </button>

          <button class="option-card" [disabled]="loading()" (click)="cargarDemo()">
            <div class="option-card__icon">🎯</div>
            <h3>Cargar datos demo</h3>
            <p>Empresa de ejemplo con productos, clientes, proveedores y empleados reales.</p>
          </button>

          <button class="option-card" [disabled]="loading()" (click)="importarCSV()">
            <div class="option-card__icon">📁</div>
            <h3>Importar desde CSV / Excel</h3>
            <p>Migrar datos existentes: productos, clientes, proveedores, empleados, stock.</p>
          </button>
        </div>

        @if (error()) { <div class="error-state">{{ error() }}</div> }
      </div>
    </div>
  `,
  styles: [`
    .onboarding {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 2rem;
      background: linear-gradient(135deg, #f5f7fa 0%, #e4ebf5 100%);
    }
    .onboarding__card {
      max-width: 960px;
      width: 100%;
      background: white;
      border-radius: 16px;
      padding: 3rem;
      box-shadow: 0 10px 40px rgba(0,0,0,0.08);
    }
    .onboarding__title { font-size: 2rem; margin: 0 0 0.5rem; color: #1a1a1a; }
    .onboarding__subtitle { color: #555; margin: 0 0 2rem; font-size: 1.1rem; }
    .onboarding__lead { font-weight: 600; margin: 0 0 1.5rem; font-size: 1.1rem; }
    .onboarding__options {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
      gap: 1.25rem;
    }
    .option-card {
      position: relative;
      background: white;
      border: 2px solid #e5e7eb;
      border-radius: 12px;
      padding: 1.75rem 1.25rem;
      text-align: left;
      cursor: pointer;
      transition: all 0.18s ease;
      font-family: inherit;
    }
    .option-card:hover:not(:disabled) {
      border-color: #3b82f6;
      transform: translateY(-2px);
      box-shadow: 0 8px 24px rgba(59,130,246,0.15);
    }
    .option-card:disabled { opacity: 0.6; cursor: wait; }
    .option-card__icon { font-size: 2.5rem; margin-bottom: 0.75rem; }
    .option-card h3 { margin: 0 0 0.5rem; font-size: 1.1rem; color: #1a1a1a; }
    .option-card p { margin: 0; color: #666; font-size: 0.9rem; line-height: 1.5; }
    .option-card__badge {
      position: absolute;
      top: 0.75rem;
      right: 0.75rem;
      background: #fef3c7;
      color: #92400e;
      font-size: 0.7rem;
      padding: 0.2rem 0.5rem;
      border-radius: 999px;
      font-weight: 600;
    }
    .error-state {
      margin-top: 1.5rem;
      padding: 0.75rem 1rem;
      background: #fee;
      color: #c00;
      border-radius: 8px;
    }
  `],
})
export class OnboardingComponent {
  private auth = inject(AuthService);
  private router = inject(Router);
  private http = inject(HttpClient);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly userName = () => this.auth.currentUser()?.name ?? '';
  readonly empresa = () => this.auth.currentUser()?.company ?? '';

  async empezarVacio(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    const ok = await this.auth.completarOnboarding();
    this.loading.set(false);
    if (ok) this.router.navigate(['/app/dashboard']);
    else this.error.set('No se pudo completar el onboarding');
  }

  async cargarDemo(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    try {
      await firstValueFrom(
        this.http.post(`${environment.apiUrl}/seguridad/empresa/cargar-datos-demo`, {})
      );
      await this.auth.completarOnboarding();
      this.router.navigate(['/app/dashboard']);
    } catch (err: any) {
      this.error.set(err?.error?.message ?? 'Error al cargar datos demo');
    } finally {
      this.loading.set(false);
    }
  }

  async importarCSV(): Promise<void> {
    this.loading.set(true);
    const ok = await this.auth.completarOnboarding();
    this.loading.set(false);
    if (ok) this.router.navigate(['/app/importador']);
    else this.error.set('No se pudo completar el onboarding');
  }
}
