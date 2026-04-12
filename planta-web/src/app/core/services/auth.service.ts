import { Injectable, signal, computed, inject } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { User, LoginCredentials, RegisterData, AuthResponse } from '../models/user.model';
import { environment } from '../../../environments/environment';

const TOKEN_KEY = 'planta_access_token';
const USER_KEY = 'planta_user';

interface BackendUserDto {
  id: string;
  email: string;
  nombre?: string;
  rol?: string;
  empresaId?: string;
  empresaNombre?: string;
  name?: string;
  role?: string;
  company?: string;
}

interface BackendAuthResponse {
  accessToken?: string;
  refreshToken?: string;
  token?: string;
  user: BackendUserDto;
}

function mapUser(dto: BackendUserDto): User {
  return {
    id: dto.id,
    email: dto.email,
    name: dto.nombre ?? dto.name ?? dto.email ?? '',
    role: (dto.rol ?? dto.role ?? 'Operario') as User['role'],
    company: dto.empresaNombre ?? dto.company ?? '',
  };
}

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
    const source = (user.name ?? user.email ?? '').trim();
    if (!source) return '';
    return source.split(/\s+/).map(n => n[0] ?? '').join('').toUpperCase().slice(0, 2);
  });

  async login(credentials: LoginCredentials): Promise<boolean> {
    this._isLoading.set(true);
    this._error.set(null);

    try {
      const response = await firstValueFrom(
        this.http.post<BackendAuthResponse>(`${this.apiUrl}/seguridad/auth/login`, {
          email: credentials.email,
          password: credentials.password,
        })
      );

      const token = response.accessToken ?? response.token ?? '';
      const user = mapUser(response.user);
      this.persistSession(token, user);
      this._currentUser.set(user);
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
        this.http.post<BackendAuthResponse>(`${this.apiUrl}/seguridad/auth/register`, {
          nombre: data.name,
          email: data.email,
          password: data.password,
          empresaNombre: data.company,
        })
      );

      const token = response.accessToken ?? response.token ?? '';
      const user = mapUser(response.user);
      this.persistSession(token, user);
      this._currentUser.set(user);
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
      if (!raw) return null;
      const parsed = JSON.parse(raw) as BackendUserDto;
      return mapUser(parsed);
    } catch {
      return null;
    }
  }
}
