import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';
import { onboardingGuard } from './core/guards/onboarding.guard';
import { roleGuard } from './core/guards/role.guard';

const ALL_ROLES = ['Administrador', 'GerentePlanta', 'JefeAlmacen', 'JefeProduccion', 'Compras', 'Ventas', 'Operario', 'Calidad'];

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
        canActivate: [roleGuard(['Administrador', 'GerentePlanta', 'JefeAlmacen', 'JefeProduccion', 'Compras', 'Operario', 'Calidad'])],
        loadChildren: () =>
          import('./features/inventario/inventario.routes').then(m => m.INVENTARIO_ROUTES),
      },
      {
        path: 'produccion',
        canActivate: [roleGuard(['Administrador', 'GerentePlanta', 'JefeProduccion', 'Operario'])],
        loadChildren: () =>
          import('./features/produccion/produccion.routes').then(m => m.PRODUCCION_ROUTES),
      },
      {
        path: 'compras',
        canActivate: [roleGuard(['Administrador', 'GerentePlanta', 'Compras'])],
        loadChildren: () =>
          import('./features/compras/compras.routes').then(m => m.COMPRAS_ROUTES),
      },
      {
        path: 'ventas',
        canActivate: [roleGuard(['Administrador', 'GerentePlanta', 'Ventas'])],
        loadChildren: () =>
          import('./features/ventas/ventas.routes').then(m => m.VENTAS_ROUTES),
      },
      {
        path: 'calidad',
        canActivate: [roleGuard(['Administrador', 'GerentePlanta', 'Calidad'])],
        loadChildren: () =>
          import('./features/calidad/calidad.routes').then(m => m.CALIDAD_ROUTES),
      },
      {
        path: 'activos',
        canActivate: [roleGuard(['Administrador', 'GerentePlanta', 'JefeProduccion'])],
        loadChildren: () =>
          import('./features/activos/activos.routes').then(m => m.ACTIVOS_ROUTES),
      },
      {
        path: 'mantenimiento',
        canActivate: [roleGuard(['Administrador', 'GerentePlanta', 'JefeProduccion', 'Operario'])],
        loadChildren: () =>
          import('./features/mantenimiento/mantenimiento.routes').then(m => m.MANTENIMIENTO_ROUTES),
      },
      {
        path: 'incidencias',
        canActivate: [roleGuard(ALL_ROLES)],
        loadChildren: () =>
          import('./features/incidencias/incidencias.routes').then(m => m.INCIDENCIAS_ROUTES),
      },
      {
        path: 'facturacion',
        canActivate: [roleGuard(['Administrador', 'GerentePlanta', 'Ventas'])],
        loadChildren: () =>
          import('./features/facturacion/facturacion.routes').then(m => m.FACTURACION_ROUTES),
      },
      {
        path: 'crm',
        canActivate: [roleGuard(['Administrador', 'GerentePlanta', 'Ventas'])],
        loadChildren: () =>
          import('./features/crm/crm.routes').then(m => m.CRM_ROUTES),
      },
      {
        path: 'rrhh',
        canActivate: [roleGuard(['Administrador', 'GerentePlanta'])],
        loadChildren: () =>
          import('./features/rrhh/rrhh.routes').then(m => m.RRHH_ROUTES),
      },
      {
        path: 'costes',
        canActivate: [roleGuard(['Administrador', 'GerentePlanta'])],
        loadChildren: () =>
          import('./features/costes/costes.routes').then(m => m.COSTES_ROUTES),
      },
      {
        path: 'oee',
        canActivate: [roleGuard(['Administrador', 'GerentePlanta', 'JefeProduccion'])],
        loadChildren: () =>
          import('./features/oee/oee.routes').then(m => m.OEE_ROUTES),
      },
      {
        path: 'importador',
        canActivate: [roleGuard(['Administrador'])],
        loadChildren: () =>
          import('./features/importador/importador.routes').then(m => m.IMPORTADOR_ROUTES),
      },
      {
        path: 'ia',
        canActivate: [roleGuard(ALL_ROLES)],
        loadChildren: () =>
          import('./features/ia/ia.routes').then(m => m.IA_ROUTES),
      },
      {
        path: 'movil',
        canActivate: [roleGuard(['Administrador', 'Operario'])],
        loadChildren: () =>
          import('./features/movil/movil.routes').then(m => m.MOVIL_ROUTES),
      },
      {
        path: 'auditoria',
        canActivate: [roleGuard(['Administrador', 'GerentePlanta'])],
        loadComponent: () =>
          import('./features/auditoria/auditoria-list.component').then(m => m.AuditoriaListComponent),
      },
      {
        path: 'permisos',
        canActivate: [roleGuard(['Administrador'])],
        loadComponent: () =>
          import('./features/permisos/permisos-list.component').then(m => m.PermisosListComponent),
      },
      {
        path: 'settings',
        loadComponent: () =>
          import('./pages/app/settings-page/settings-page.component').then(
            m => m.SettingsPageComponent
          ),
      },
      {
        path: 'forbidden',
        loadComponent: () =>
          import('./pages/app/not-found/not-found.component').then(m => m.NotFoundComponent),
      },
      {
        path: '**',
        loadComponent: () =>
          import('./pages/app/not-found/not-found.component').then(m => m.NotFoundComponent),
      },
    ],
  },
  {
    path: '**',
    loadComponent: () =>
      import('./pages/app/not-found/not-found.component').then(m => m.NotFoundComponent),
  },
];
