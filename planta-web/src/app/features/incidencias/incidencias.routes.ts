import { Routes } from '@angular/router';

export const INCIDENCIAS_ROUTES: Routes = [
  { path: '', loadComponent: () => import('./incidencias-list.component').then(m => m.IncidenciasListComponent) },
  { path: ':id', loadComponent: () => import('./incidencia-detail.component').then(m => m.IncidenciaDetailComponent) },
];
