export interface User {
  id: string;
  email: string;
  name: string;
  role: 'Administrador' | 'GerentePlanta' | 'JefeAlmacen' | 'JefeProduccion' | 'Compras' | 'Ventas' | 'Operario' | 'Calidad';
  company: string;
  avatar?: string;
  onboardingCompletado?: boolean;
  trialHasta?: string | null;
  modulosDeshabilitados?: string[];
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface LoginCredentials {
  email: string;
  password: string;
}

export interface RegisterData {
  name: string;
  email: string;
  password: string;
  company: string;
}
