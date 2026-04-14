import { Routes } from '@angular/router';

export const COSTES_ROUTES: Routes = [
  { path: '', redirectTo: 'imputaciones', pathMatch: 'full' },
  {
    path: 'imputaciones',
    loadComponent: () => import('./imputaciones-list.component').then(m => m.ImputacionesListComponent),
  },
];
