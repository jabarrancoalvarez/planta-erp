import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';

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
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
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
