import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { ProduccionService, BOMDetailDto } from '../../../core/services/produccion.service';

@Component({
  selector: 'app-bom-detail',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="detail-page">
      <div class="detail-page__header">
        <button class="detail-page__back" (click)="goBack()">&larr; Volver</button>
        <h1 class="detail-page__title">Detalle de BOM</h1>
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando BOM...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else if (item(); as bom) {
        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Informacion General</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Nombre</span>
              <span class="detail-page__field-value">{{ bom.nombre }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Version</span>
              <span class="detail-page__field-value"><code>v{{ bom.versionBOM }}</code></span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Estado</span>
              <span class="detail-page__field-value">
                <span class="active-dot" [class.active-dot--active]="bom.activo" [class.active-dot--inactive]="!bom.activo"></span>
                {{ bom.activo ? 'Activa' : 'Inactiva' }}
              </span>
            </div>
          </div>
        </div>

        @if (bom.descripcion) {
          <div class="detail-page__section">
            <h2 class="detail-page__section-title">Descripcion</h2>
            <p>{{ bom.descripcion }}</p>
          </div>
        }

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Lineas de Material ({{ bom.lineas.length }})</h2>
          @if (bom.lineas.length > 0) {
            <div class="table-container">
              <table class="data-table">
                <thead>
                  <tr>
                    <th>Componente</th>
                    <th>Cantidad</th>
                    <th>Unidad</th>
                    <th>Merma Est.</th>
                  </tr>
                </thead>
                <tbody>
                  @for (line of bom.lineas; track line.id) {
                    <tr>
                      <td>{{ line.componenteNombre }}</td>
                      <td>{{ line.cantidad }}</td>
                      <td>{{ line.unidadMedida }}</td>
                      <td>{{ line.mermaEstimada }}%</td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          } @else {
            <p class="empty-state">Sin lineas de material definidas</p>
          }
        </div>
      }
    </div>
  `,
})
export class BOMDetailComponent implements OnInit {
  private svc = inject(ProduccionService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  readonly item = signal<BOMDetailDto | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.loading.set(true);
    this.svc.getBOM(id).subscribe({
      next: (data) => { this.item.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar BOM'); this.loading.set(false); },
    });
  }

  goBack(): void { this.router.navigate(['/app/produccion/bom']); }
}
