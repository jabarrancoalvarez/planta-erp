import { Routes } from '@angular/router';

export const FACTURACION_ROUTES: Routes = [
  { path: '', redirectTo: 'facturas', pathMatch: 'full' },
  {
    path: 'facturas',
    loadComponent: () => import('./facturas-list.component').then(m => m.FacturasListComponent),
  },
];
