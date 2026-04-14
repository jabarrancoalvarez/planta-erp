import { Routes } from '@angular/router';

export const INVENTARIO_ROUTES: Routes = [
  { path: '', redirectTo: 'productos', pathMatch: 'full' },
  { path: 'productos', loadComponent: () => import('./productos/productos-list.component').then(m => m.ProductosListComponent) },
  { path: 'productos/:id', loadComponent: () => import('./productos/producto-detail.component').then(m => m.ProductoDetailComponent) },
  { path: 'almacenes', loadComponent: () => import('./almacenes/almacenes-list.component').then(m => m.AlmacenesListComponent) },
  { path: 'almacenes/:id', loadComponent: () => import('./almacenes/almacen-detail.component').then(m => m.AlmacenDetailComponent) },
  { path: 'movimientos', loadComponent: () => import('./movimientos/movimientos-list.component').then(m => m.MovimientosListComponent) },
  { path: 'lotes', loadComponent: () => import('./lotes/lotes-list.component').then(m => m.LotesListComponent) },
  { path: 'alertas', loadComponent: () => import('./alertas/alertas-list.component').then(m => m.AlertasListComponent) },
];
