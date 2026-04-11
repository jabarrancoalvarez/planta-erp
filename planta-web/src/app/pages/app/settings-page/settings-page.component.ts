import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';

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
}
