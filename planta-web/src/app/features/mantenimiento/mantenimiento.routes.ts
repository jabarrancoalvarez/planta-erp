import { Routes } from '@angular/router';

export const MANTENIMIENTO_ROUTES: Routes = [
  { path: '', loadComponent: () => import('./ordenes-list.component').then(m => m.OrdenesListComponent) },
];
