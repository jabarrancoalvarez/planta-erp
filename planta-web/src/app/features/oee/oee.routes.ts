import { Routes } from '@angular/router';

export const OEE_ROUTES: Routes = [
  { path: '', redirectTo: 'registros', pathMatch: 'full' },
  {
    path: 'registros',
    loadComponent: () => import('./oee-list.component').then(m => m.OeeListComponent),
  },
];
