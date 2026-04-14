import { Component, signal, inject, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ImportadorService, ImportJobDto, TipoImportacion, FormatoArchivo } from '../../core/services/importador.service';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-import-jobs-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, FormsModule, DatePipe],
  template: `
    <div class="list-page">
      <div class="list-page__header">
        <h1 class="list-page__title">Jobs de Importacion</h1>
        <button class="btn-primary" (click)="toggleCreate()">{{ showCreate() ? 'Cancelar' : '+ Nuevo job' }}</button>
      </div>

      @if (showCreate()) {
        <div class="detail-page__section">
          <h2 class="detail-page__section-title">Crear job</h2>
          <div class="detail-page__grid">
            <div class="detail-page__field">
              <label class="detail-page__field-label">Tipo *</label>
              <select [(ngModel)]="newTipo">
                @for (t of tipos; track t) { <option [value]="t">{{ t }}</option> }
              </select>
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Formato *</label>
              <select [(ngModel)]="newFormato">
                @for (f of formatos; track f) { <option [value]="f">{{ f }}</option> }
              </select>
            </div>
            <div class="detail-page__field">
              <label class="detail-page__field-label">Nombre archivo *</label>
              <input [(ngModel)]="newNombre" placeholder="clientes.csv" />
            </div>
          </div>
          @if (createError()) { <div class="error-state">{{ createError() }}</div> }
          <div style="display:flex; gap:0.5rem; margin-top:1rem;">
            <button class="btn-primary" (click)="save()" [disabled]="saving()">{{ saving() ? 'Creando...' : 'Crear' }}</button>
          </div>
        </div>
      }

      @if (loading()) {
        <div class="loading-state">Cargando...</div>
      } @else if (error()) {
        <div class="error-state">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="data-table">
            <thead>
              <tr>
                <th>Archivo</th>
                <th>Tipo</th>
                <th>Formato</th>
                <th>Estado</th>
                <th>Total/Validas/Error/Import.</th>
                <th>Iniciado</th>
                <th>Finalizado</th>
              </tr>
            </thead>
            <tbody>
              @for (j of items(); track j.id) {
                <tr>
                  <td><code>{{ j.nombreArchivo }}</code></td>
                  <td>{{ j.tipo }}</td>
                  <td>{{ j.formato }}</td>
                  <td><span class="badge">{{ j.estado }}</span></td>
                  <td>{{ j.filasTotales }}/{{ j.filasValidas }}/{{ j.filasConError }}/{{ j.filasImportadas }}</td>
                  <td>{{ j.iniciadoEn ? (j.iniciadoEn | date:'dd/MM HH:mm') : '---' }}</td>
                  <td>{{ j.finalizadoEn ? (j.finalizadoEn | date:'dd/MM HH:mm') : '---' }}</td>
                </tr>
              } @empty {
                <tr><td colspan="7" class="empty-state">Sin jobs</td></tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `,
})
export class ImportJobsListComponent implements OnInit {
  private svc = inject(ImportadorService);
  private auth = inject(AuthService);

  readonly items = signal<ImportJobDto[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly showCreate = signal(false);
  readonly saving = signal(false);
  readonly createError = signal<string | null>(null);

  tipos: TipoImportacion[] = ['Productos', 'Clientes', 'Proveedores', 'Empleados', 'Activos', 'Stock'];
  formatos: FormatoArchivo[] = ['Csv', 'Xlsx'];

  newTipo: TipoImportacion = 'Productos';
  newFormato: FormatoArchivo = 'Csv';
  newNombre = '';

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.svc.listJobs().subscribe({
      next: (r) => { this.items.set(r.items); this.loading.set(false); },
      error: (err) => { this.error.set(err?.error?.message ?? 'Error al cargar'); this.loading.set(false); },
    });
  }

  toggleCreate(): void {
    this.showCreate.update(v => !v);
    this.createError.set(null);
  }

  save(): void {
    if (!this.newNombre) {
      this.createError.set('Nombre archivo obligatorio');
      return;
    }
    this.saving.set(true);
    this.svc.createJob({
      tipo: this.newTipo,
      formato: this.newFormato,
      nombreArchivo: this.newNombre,
      userId: this.auth.currentUser()?.id ?? '00000000-0000-0000-0000-000000000000',
    }).subscribe({
      next: () => {
        this.saving.set(false);
        this.showCreate.set(false);
        this.newNombre = '';
        this.load();
      },
      error: (err) => {
        this.createError.set(err?.error?.message ?? 'Error al crear');
        this.saving.set(false);
      },
    });
  }
}
