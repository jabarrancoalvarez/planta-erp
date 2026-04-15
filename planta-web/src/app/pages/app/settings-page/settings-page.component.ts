import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-settings-page',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './settings-page.component.html',
  styleUrl: './settings-page.component.scss',
})
export class SettingsPageComponent {
  private fb = inject(FormBuilder);
  readonly auth = inject(AuthService);

  activeTab = signal<'profile' | 'company' | 'notifications' | 'billing'>('profile');

  setActiveTab(t: string): void {
    this.activeTab.set(t as 'profile' | 'company' | 'notifications' | 'billing');
  }
  saveSuccess = signal(false);

  profileForm = this.fb.group({
    name: [this.auth.currentUser()?.name ?? ''],
    email: [this.auth.currentUser()?.email ?? ''],
    role: [this.auth.currentUser()?.role ?? ''],
    phone: ['+34 600 000 000'],
  });

  companyForm = this.fb.group({
    company: [this.auth.currentUser()?.company ?? ''],
    sector: ['Industria manufacturera'],
    size: ['11-50 empleados'],
    address: ['Calle Industrial 12, Barcelona'],
  });

  notifForm = this.fb.group({
    emailIncidents: [true],
    emailMaintenance: [true],
    emailReports: [false],
    pushIncidents: [true],
    pushMaintenance: [true],
  });

  onSave(): void {
    this.saveSuccess.set(true);
    setTimeout(() => this.saveSuccess.set(false), 3000);
  }

  private api = inject(ApiService);
  private router = inject(Router);
  readonly gdprLoading = signal(false);

  exportarDatos(): void {
    this.gdprLoading.set(true);
    this.api.getBlob('/gdpr/export').subscribe({
      next: (blob) => {
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `planta-export-${new Date().toISOString().slice(0, 10)}.zip`;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
        this.gdprLoading.set(false);
      },
      error: () => {
        this.gdprLoading.set(false);
        alert('Error al exportar datos');
      },
    });
  }

  eliminarCuenta(): void {
    const confirmText = 'ELIMINAR';
    const input = prompt(`Esta acción eliminará tu cuenta permanentemente.\n\nEscribe "${confirmText}" para confirmar:`);
    if (input !== confirmText) return;
    this.gdprLoading.set(true);
    this.api.delete('/gdpr/account').subscribe({
      next: () => {
        this.gdprLoading.set(false);
        this.auth.logout();
        this.router.navigate(['/']);
      },
      error: () => {
        this.gdprLoading.set(false);
        alert('Error al eliminar cuenta');
      },
    });
  }
}
