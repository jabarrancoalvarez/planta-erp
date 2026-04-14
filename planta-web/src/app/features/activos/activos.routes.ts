import { Routes } from '@angular/router';

export const ACTIVOS_ROUTES: Routes = [
  { path: '', loadComponent: () => import('./activos-list.component').then(m => m.ActivosListComponent) },
];
