import { Routes } from '@angular/router';

export const PRODUCCION_ROUTES: Routes = [
  { path: '', redirectTo: 'ordenes', pathMatch: 'full' },
  { path: 'bom', loadComponent: () => import('./bom/bom-list.component').then(m => m.BomListComponent) },
  { path: 'bom/:id', loadComponent: () => import('./bom/bom-detail.component').then(m => m.BOMDetailComponent) },
  { path: 'rutas', loadComponent: () => import('./rutas/rutas-list.component').then(m => m.RutasListComponent) },
  { path: 'ordenes', loadComponent: () => import('./ordenes/ordenes-list.component').then(m => m.OrdenesListComponent) },
  { path: 'ordenes/:id', loadComponent: () => import('./ordenes/of-detail.component').then(m => m.OFDetailComponent) },
];
