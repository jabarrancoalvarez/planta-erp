import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ProduccionService, BOMListDto } from '../../../core/services/produccion.service';
import { NotificationService } from '../../../shared/components/toast/notification.service';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-bom-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ReactiveFormsModule, PaginationComponent],
  template: `
    <div class="page">
      <div class="page__header">
        <div>
          <h1 class="page__title">Listas de Materiales (BOM)</h1>
          <p class="page__subtitle">Definicion de componentes por producto</p>
        </div>
        <button class="btn-primary" (click)="showForm.set(true)">+ Nueva BOM</button>
      </div>

      <div class="page__filters">
        <input class="filter-input" placeholder="Buscar por nombre..."
               (input)="onSearch($event)" />
      </div>

      @if (loading()) {
        <div class="loading-state">Cargando BOMs...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Nombre</th>
                <th>Version</th>
                <th>Num. Lineas</th>
                <th>Estado</th>
              </tr>
            </thead>
            <tbody>
              @for (item of items(); track item.id) {
                <tr class="clickable-row" (click)="goToDetail(item.id)">
                  <td>{{ item.nombre }}</td>
                  <td><code>v{{ item.versionBOM }}</code></td>
                  <td>{{ item.numeroLineas }}</td>
                  <td>
                    <span class="active-dot" [class.active-dot--active]="item.activo" [class.active-dot--inactive]="!item.activo"></span>
                    {{ item.activo ? 'Activa' : 'Inactiva' }}
                  </td>
                </tr>
              } @empty {
                <tr><td colspan="4" class="empty-state">No se encontraron resultados</td></tr>
              }
            </tbody>
          </table>
        </div>

        @if (totalPages() > 1) {
          <app-pagination [currentPage]="currentPage()" [totalPages]="totalPages()" (pageChange)="onPageChange($event)" />
        }

        <div class="page__footer">{{ items().length }} de {{ totalCount() }} registros</div>
      }
    </div>

    @if (showForm()) {
      <div class="form-overlay" (click)="closeForm()">
        <div class="form-dialog" (click)="$event.stopPropagation()">
          <h2 class="form-dialog__title">Nueva Lista de Materiales</h2>
          <form [formGroup]="form" (ngSubmit)="onSubmit()">
            <div class="form-group">
              <label class="form-label">Nombre *</label>
              <input class="form-input" formControlName="nombre" />
            </div>
            <div class="form-group">
              <label class="form-label">Producto ID (GUID) *</label>
              <input class="form-input" formControlName="productoId" placeholder="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx" />
            </div>
            <div class="form-group">
              <label class="form-label">Descripcion</label>
              <textarea class="form-textarea" formControlName="descripcion"></textarea>
            </div>
            <div class="form-actions">
              <button type="button" class="btn-outline" (click)="closeForm()">Cancelar</button>
              <button type="submit" class="btn-primary" [disabled]="form.invalid || submitting()">
                {{ submitting() ? 'Creando...' : 'Crear BOM' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    }
  `,
})
export class BomListComponent implements OnInit {
  private svc = inject(ProduccionService);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private notify = inject(NotificationService);

  readonly items = signal<BOMListDto[]>([]);
  readonly totalCount = signal(0);
  readonly currentPage = signal(1);
  readonly totalPages = signal(1);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly showForm = signal(false);
  readonly submitting = signal(false);
  private searchTimeout: any;
  private currentSearch = '';

  readonly form = this.fb.nonNullable.group({
    nombre: ['', Validators.required],
    productoId: ['', Validators.required],
    descripcion: [''],
  });

  ngOnInit(): void { this.load(); }

  onSearch(event: Event): void {
    clearTimeout(this.searchTimeout);
    this.currentSearch = (event.target as HTMLInputElement).value;
    this.searchTimeout = setTimeout(() => { this.currentPage.set(1); this.load(); }, 300);
  }

  onPageChange(page: number): void { this.currentPage.set(page); this.load(); }
  goToDetail(id: string): void { this.router.navigate(['/app/produccion/bom', id]); }
  closeForm(): void { this.showForm.set(false); this.form.reset(); }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.submitting.set(true);
    const v = this.form.getRawValue();
    this.svc.createBOM({ nombre: v.nombre, productoId: v.productoId, descripcion: v.descripcion || undefined }).subscribe({
      next: () => { this.notify.success('BOM creada correctamente'); this.closeForm(); this.submitting.set(false); this.load(); },
      error: (err) => { this.notify.error(err?.error?.message ?? 'Error al crear BOM'); this.submitting.set(false); },
    });
  }

  private load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.svc.listBOMs(this.currentSearch || undefined, this.currentPage()).subscribe({
      next: (res) => { this.items.set(res.items); this.totalCount.set(res.totalCount); this.totalPages.set(res.totalPages); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar BOMs'); this.loading.set(false); },
    });
  }
}
