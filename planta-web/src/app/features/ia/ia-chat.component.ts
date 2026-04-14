import { Component, signal, inject, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IaService, ContextoIA, MensajeIADto } from '../../core/services/ia.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-ia-chat',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="list-page">
      <div class="list-page__header">
        <h1 class="list-page__title">Asistente IA</h1>
      </div>

      <div class="detail-page__section">
        <div class="detail-page__grid">
          <div class="detail-page__field">
            <label class="detail-page__field-label">Contexto</label>
            <select [(ngModel)]="contexto">
              @for (c of contextos; track c) { <option [value]="c">{{ c }}</option> }
            </select>
          </div>
        </div>
      </div>

      <div class="detail-page__section">
        <h2 class="detail-page__section-title">Conversacion</h2>
        @if (mensajes().length === 0) {
          <p class="empty-state">Sin mensajes. Escribe abajo para empezar.</p>
        }
        @for (m of mensajes(); track m.id) {
          <div style="margin-bottom:1rem; padding:0.75rem; border-radius:0.5rem;"
               [style.background]="m.rol === 'User' ? '#eef' : '#efe'">
            <strong>{{ m.rol }}:</strong>
            <div style="white-space:pre-wrap;">{{ m.contenido }}</div>
          </div>
        }
        @if (sending()) {
          <div class="loading-state">Enviando...</div>
        }
        @if (error()) { <div class="error-state">{{ error() }}</div> }

        <div style="display:flex; gap:0.5rem; margin-top:1rem;">
          <input [(ngModel)]="input" (keyup.enter)="send()" placeholder="Escribe un mensaje..." style="flex:1;" />
          <button class="btn-primary" (click)="send()" [disabled]="sending() || !input.trim()">Enviar</button>
        </div>
      </div>
    </div>
  `,
})
export class IaChatComponent {
  private svc = inject(IaService);
  private auth = inject(AuthService);

  readonly mensajes = signal<MensajeIADto[]>([]);
  readonly sending = signal(false);
  readonly error = signal<string | null>(null);
  readonly conversacionId = signal<string | null>(null);

  contextos: ContextoIA[] = ['General', 'Inventario', 'Produccion', 'Compras', 'Ventas', 'Calidad', 'Mantenimiento', 'RRHH', 'Facturacion'];
  contexto: ContextoIA = 'General';
  input = '';

  send(): void {
    const texto = this.input.trim();
    if (!texto) return;
    this.sending.set(true);
    this.error.set(null);

    const userMsg: MensajeIADto = {
      id: crypto.randomUUID(),
      rol: 'User',
      contenido: texto,
      tokensEntrada: null,
      tokensSalida: null,
      modelo: null,
      createdAt: new Date().toISOString(),
    };
    this.mensajes.update(arr => [...arr, userMsg]);
    this.input = '';

    this.svc.enviar({
      conversacionId: this.conversacionId() ?? undefined,
      mensaje: texto,
      usuarioId: this.auth.currentUser()?.id ?? '00000000-0000-0000-0000-000000000000',
      contexto: this.contexto,
    }).subscribe({
      next: (r) => {
        this.conversacionId.set(r.conversacionId);
        this.mensajes.update(arr => [...arr, {
          id: crypto.randomUUID(),
          rol: 'Assistant',
          contenido: r.respuesta,
          tokensEntrada: r.tokensEntrada,
          tokensSalida: r.tokensSalida,
          modelo: r.modelo,
          createdAt: new Date().toISOString(),
        }]);
        this.sending.set(false);
      },
      error: (err) => {
        this.error.set(err?.error?.message ?? 'Error al enviar mensaje');
        this.sending.set(false);
      },
    });
  }
}
