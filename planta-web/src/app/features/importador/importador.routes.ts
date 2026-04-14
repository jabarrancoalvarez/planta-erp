import { Routes } from '@angular/router';

export const IMPORTADOR_ROUTES: Routes = [
  { path: '', redirectTo: 'jobs', pathMatch: 'full' },
  {
    path: 'jobs',
    loadComponent: () => import('./jobs-list.component').then(m => m.ImportJobsListComponent),
  },
];
