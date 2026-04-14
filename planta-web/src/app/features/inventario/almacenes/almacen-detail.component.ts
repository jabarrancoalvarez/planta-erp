import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { InventarioService, AlmacenDetailDto } from '../../../core/services/inventario.service';

@Component({
  selector: 'app-almacen-detail',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule],
  template: `
    <div class="detail-page">
      <div class="detail-page__header">
        <button class="detail-page__back" (click)="goBack()">&larr; Volver</button>
        <h1 class="detail-page__title">Detalle de Almacén</h1>
        @if (item()) {
          <button class="detail-page__back" style="margin-left:auto;background:#fee;color:#c00;" (click)="onDelete()">Eliminar</button>
        }
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando almacén...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else if (item(); as a) {
        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Datos Generales</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <span class="detail-page__field-label">Nombre</span>
              <span class="detail-page__field-value">{{ a.nombre }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Dirección</span>
              <span class="detail-page__field-value">{{ a.direccion ?? '---' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Descripción</span>
              <span class="detail-page__field-value">{{ a.descripcion ?? '---' }}</span>
            </div>
            <div class="detail-page__field">
              <span class="detail-page__field-label">Principal</span>
              <span class="detail-page__field-value">{{ a.esPrincipal ? 'Sí' : 'No' }}</span>
            </div>
          </div>
        </div>

        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Ubicaciones ({{ a.ubicaciones.length }})</h2>
          @if (a.ubicaciones.length > 0) {
            <div class="table-container">
              <table class="data-table">
                <thead>
                  <tr>
                    <th>Código</th>
                    <th>Nombre</th>
                    <th>Capacidad</th>
                    <th>Estado</th>
                  </tr>
                </thead>
                <tbody>
                  @for (u of a.ubicaciones; track u.id) {
                    <tr>
                      <td><code>{{ u.codigo }}</code></td>
                      <td>{{ u.nombre ?? '---' }}</td>
                      <td>{{ u.capacidadMaxima }}</td>
                      <td>{{ u.activa ? 'Activa' : 'Inactiva' }}</td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          } @else {
            <div class="empty-state">Sin ubicaciones</div>
          }
        </div>
      }
    </div>
  `,
})
export class AlmacenDetailComponent implements OnInit {
  private svc = inject(InventarioService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  readonly item = signal<AlmacenDetailDto | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.loading.set(true);
    this.svc.getAlmacen(id).subscribe({
      next: (data) => { this.item.set(data); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar almacén'); this.loading.set(false); },
    });
  }

  goBack(): void { this.router.navigate(['/app/inventario/almacenes']); }

  onDelete(): void {
    const a = this.item();
    if (!a) return;
    if (!confirm(`¿Eliminar almacén "${a.nombre}"?`)) return;
    this.svc.deleteAlmacen(a.id).subscribe({
      next: () => this.router.navigate(['/app/inventario/almacenes']),
      error: (err) => this.error.set(err?.error?.message ?? 'Error al eliminar'),
    });
  }
}
