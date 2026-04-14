import { Routes } from '@angular/router';

export const RRHH_ROUTES: Routes = [
  { path: '', redirectTo: 'empleados', pathMatch: 'full' },
  {
    path: 'empleados',
    loadComponent: () => import('./empleados-list.component').then(m => m.EmpleadosListComponent),
  },
  {
    path: 'fichajes',
    loadComponent: () => import('./fichajes-list.component').then(m => m.FichajesListComponent),
  },
  {
    path: 'ausencias',
    loadComponent: () => import('./ausencias-list.component').then(m => m.AusenciasListComponent),
  },
];
