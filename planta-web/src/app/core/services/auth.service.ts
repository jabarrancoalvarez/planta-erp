import { Injectable, signal, computed, inject } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { User, LoginCredentials, RegisterData, AuthResponse } from '../models/user.model';
import { environment } from '../../../environments/environment';

const TOKEN_KEY = 'planta_access_token';
const USER_KEY = 'planta_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private router = inject(Router);
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  private _currentUser = signal<User | null>(this.loadUserFromStorage());
  private _isLoading = signal(false);
  private _error = signal<string | null>(null);

  readonly currentUser = this._currentUser.asReadonly();
  readonly isLoading = this._isLoading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly isAuthenticated = computed(() => this._currentUser() !== null);
  readonly userInitials = computed(() => {
    const user = this._currentUser();
    if (!user) return '';
    return user.name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  });

  async login(credentials: LoginCredentials): Promise<boolean> {
    this._isLoading.set(true);
    this._error.set(null);

    try {
      const response = await firstValueFrom(
        this.http.post<AuthResponse>(`${this.apiUrl}/seguridad/auth/login`, {
          email: credentials.email,
          password: credentials.password,
        })
      );

      this.persistSession(response.token, response.user);
      this._currentUser.set(response.user);
      return true;
    } catch (err: any) {
      const message = err?.error?.message ?? err?.message ?? 'Error al iniciar sesion';
      this._error.set(message);
      return false;
    } finally {
      this._isLoading.set(false);
    }
  }

  async register(data: RegisterData): Promise<boolean> {
    this._isLoading.set(true);
    this._error.set(null);

    try {
      const response = await firstValueFrom(
        this.http.post<AuthResponse>(`${this.apiUrl}/seguridad/auth/register`, {
          name: data.name,
          email: data.email,
          password: data.password,
          company: data.company,
        })
      );

      this.persistSession(response.token, response.user);
      this._currentUser.set(response.user);
      return true;
    } catch (err: any) {
      const message = err?.error?.message ?? err?.message ?? 'Error al crear la cuenta';
      this._error.set(message);
      return false;
    } finally {
      this._isLoading.set(false);
    }
  }

  async sendPasswordReset(email: string): Promise<boolean> {
    this._isLoading.set(true);
    this._error.set(null);

    try {
      await firstValueFrom(
        this.http.post(`${this.apiUrl}/seguridad/auth/forgot-password`, { email })
      );
      return true;
    } catch (err: any) {
      const message = err?.error?.message ?? err?.message ?? 'Error al enviar el email';
      this._error.set(message);
      return false;
    } finally {
      this._isLoading.set(false);
    }
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this._currentUser.set(null);
    this._error.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  clearError(): void {
    this._error.set(null);
  }

  private persistSession(token: string, user: User): void {
    localStorage.setItem(TOKEN_KEY, token);
    localStorage.setItem(USER_KEY, JSON.stringify(user));
  }

  private loadUserFromStorage(): User | null {
    try {
      const raw = localStorage.getItem(USER_KEY);
      return raw ? (JSON.parse(raw) as User) : null;
    } catch {
      return null;
    }
  }
}
