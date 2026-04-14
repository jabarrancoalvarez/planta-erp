import { Routes } from '@angular/router';

export const MANTENIMIENTO_ROUTES: Routes = [
  { path: '', loadComponent: () => import('./ordenes-list.component').then(m => m.OrdenesListComponent) },
  { path: 'ordenes/:id', loadComponent: () => import('./orden-detail.component').then(m => m.OrdenDetailComponent) },
];
