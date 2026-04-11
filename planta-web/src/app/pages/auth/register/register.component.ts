import { Component, inject, OnDestroy } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators, AbstractControl } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

function passwordMatch(control: AbstractControl) {
  const password = control.get('password')?.value;
  const confirm = control.get('confirmPassword')?.value;
  return password === confirm ? null : { passwordMismatch: true };
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, CommonModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent implements OnDestroy {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  readonly isLoading = this.auth.isLoading;
  readonly serverError = this.auth.error;

  showPassword = false;
  currentStep = 1;

  form = this.fb.group(
    {
      name: ['', [Validators.required, Validators.minLength(2)]],
      company: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required],
      terms: [false, Validators.requiredTrue],
    },
    { validators: passwordMatch }
  );

  get name() { return this.form.controls.name; }
  get company() { return this.form.controls.company; }
  get email() { return this.form.controls.email; }
  get password() { return this.form.controls.password; }
  get confirmPassword() { return this.form.controls.confirmPassword; }
  get terms() { return this.form.controls.terms; }

  passwordStrength(): 'weak' | 'medium' | 'strong' {
    const val = this.password.value ?? '';
    if (val.length < 8) return 'weak';
    const hasUpper = /[A-Z]/.test(val);
    const hasNumber = /\d/.test(val);
    const hasSpecial = /[^A-Za-z0-9]/.test(val);
    const score = [hasUpper, hasNumber, hasSpecial].filter(Boolean).length;
    return score >= 2 ? 'strong' : 'medium';
  }

  async onSubmit(): Promise<void> {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const ok = await this.auth.register({
      name: this.name.value!,
      email: this.email.value!,
      password: this.password.value!,
      company: this.company.value!,
    });

    if (ok) {
      this.router.navigate(['/app/dashboard']);
    }
  }

  ngOnDestroy(): void {
    this.auth.clearError();
  }
}
