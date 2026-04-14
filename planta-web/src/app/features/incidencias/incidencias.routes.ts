import { Routes } from '@angular/router';

export const INCIDENCIAS_ROUTES: Routes = [
  { path: '', loadComponent: () => import('./incidencias-list.component').then(m => m.IncidenciasListComponent) },
];
