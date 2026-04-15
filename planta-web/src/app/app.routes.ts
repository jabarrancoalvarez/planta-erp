import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';
import { onboardingGuard } from './core/guards/onboarding.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/landing/landing.component').then(m => m.LandingComponent),
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./pages/auth/login/login.component').then(m => m.LoginComponent),
    canActivate: [guestGuard],
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./pages/auth/register/register.component').then(m => m.RegisterComponent),
    canActivate: [guestGuard],
  },
  {
    path: 'forgot-password',
    loadComponent: () =>
      import('./pages/auth/forgot-password/forgot-password.component').then(
        m => m.ForgotPasswordComponent
      ),
    canActivate: [guestGuard],
  },
  {
    path: 'app',
    loadComponent: () =>
      import('./pages/app/app-shell/app-shell.component').then(m => m.AppShellComponent),
    canActivate: [authGuard],
    canActivateChild: [onboardingGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'onboarding',
        loadComponent: () =>
          import('./pages/app/onboarding/onboarding.component').then(m => m.OnboardingComponent),
      },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./pages/app/dashboard/dashboard.component').then(m => m.DashboardComponent),
      },
      {
        path: 'inventario',
        loadChildren: () =>
          import('./features/inventario/inventario.routes').then(m => m.INVENTARIO_ROUTES),
      },
      {
        path: 'produccion',
        loadChildren: () =>
          import('./features/produccion/produccion.routes').then(m => m.PRODUCCION_ROUTES),
      },
      {
        path: 'compras',
        loadChildren: () =>
          import('./features/compras/compras.routes').then(m => m.COMPRAS_ROUTES),
      },
      {
        path: 'ventas',
        loadChildren: () =>
          import('./features/ventas/ventas.routes').then(m => m.VENTAS_ROUTES),
      },
      {
        path: 'calidad',
        loadChildren: () =>
          import('./features/calidad/calidad.routes').then(m => m.CALIDAD_ROUTES),
      },
      {
        path: 'activos',
        loadChildren: () =>
          import('./features/activos/activos.routes').then(m => m.ACTIVOS_ROUTES),
      },
      {
        path: 'mantenimiento',
        loadChildren: () =>
          import('./features/mantenimiento/mantenimiento.routes').then(m => m.MANTENIMIENTO_ROUTES),
      },
      {
        path: 'incidencias',
        loadChildren: () =>
          import('./features/incidencias/incidencias.routes').then(m => m.INCIDENCIAS_ROUTES),
      },
      {
        path: 'facturacion',
        loadChildren: () =>
          import('./features/facturacion/facturacion.routes').then(m => m.FACTURACION_ROUTES),
      },
      {
        path: 'crm',
        loadChildren: () =>
          import('./features/crm/crm.routes').then(m => m.CRM_ROUTES),
      },
      {
        path: 'rrhh',
        loadChildren: () =>
          import('./features/rrhh/rrhh.routes').then(m => m.RRHH_ROUTES),
      },
      {
        path: 'costes',
        loadChildren: () =>
          import('./features/costes/costes.routes').then(m => m.COSTES_ROUTES),
      },
      {
        path: 'oee',
        loadChildren: () =>
          import('./features/oee/oee.routes').then(m => m.OEE_ROUTES),
      },
      {
        path: 'importador',
        loadChildren: () =>
          import('./features/importador/importador.routes').then(m => m.IMPORTADOR_ROUTES),
      },
      {
        path: 'ia',
        loadChildren: () =>
          import('./features/ia/ia.routes').then(m => m.IA_ROUTES),
      },
      {
        path: 'movil',
        loadChildren: () =>
          import('./features/movil/movil.routes').then(m => m.MOVIL_ROUTES),
      },
      {
        path: 'auditoria',
        loadComponent: () =>
          import('./features/auditoria/auditoria-list.component').then(m => m.AuditoriaListComponent),
      },
      {
        path: 'settings',
        loadComponent: () =>
          import('./pages/app/settings-page/settings-page.component').then(
            m => m.SettingsPageComponent
          ),
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
