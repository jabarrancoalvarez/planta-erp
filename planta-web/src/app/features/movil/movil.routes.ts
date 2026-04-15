import { Routes } from '@angular/router';

export const MOVIL_ROUTES: Routes = [
  { path: '', loadComponent: () => import('./movil-home.component').then(m => m.MovilHomeComponent) },
  { path: 'fichar', loadComponent: () => import('./fichar.component').then(m => m.FicharComponent) },
  { path: 'mis-ordenes', loadComponent: () => import('./mis-ordenes.component').then(m => m.MisOrdenesComponent) },
  { path: 'incidencia-nueva', loadComponent: () => import('./incidencia-form.component').then(m => m.IncidenciaFormComponent) },
  { path: 'incidencias', loadComponent: () => import('./incidencias-list.component').then(m => m.IncidenciasListComponent) },
  { path: 'activos', loadComponent: () => import('./activos-list-movil.component').then(m => m.ActivosListMovilComponent) },
];
