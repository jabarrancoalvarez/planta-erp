import { Routes } from '@angular/router';

export const CRM_ROUTES: Routes = [
  { path: '', redirectTo: 'leads', pathMatch: 'full' },
  {
    path: 'leads',
    loadComponent: () => import('./leads-list.component').then(m => m.LeadsListComponent),
  },
  {
    path: 'oportunidades',
    loadComponent: () => import('./oportunidades-list.component').then(m => m.OportunidadesListComponent),
  },
];
