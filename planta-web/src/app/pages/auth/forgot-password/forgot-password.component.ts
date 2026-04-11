import { Component, inject, signal, OnDestroy } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, CommonModule],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.scss',
})
export class ForgotPasswordComponent implements OnDestroy {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);

  readonly isLoading = this.auth.isLoading;
  readonly serverError = this.auth.error;
  readonly emailSent = signal(false);

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
  });

  get email() { return this.form.controls.email; }

  async onSubmit(): Promise<void> {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const ok = await this.auth.sendPasswordReset(this.email.value!);
    if (ok) {
      this.emailSent.set(true);
    }
  }

  ngOnDestroy(): void {
    this.auth.clearError();
  }
}
