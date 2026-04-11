import { Routes } from '@angular/router';

export const COMPRAS_ROUTES: Routes = [
  { path: '', redirectTo: 'ordenes', pathMatch: 'full' },
  { path: 'proveedores', loadComponent: () => import('./proveedores/proveedores-list.component').then(m => m.ProveedoresListComponent) },
  { path: 'proveedores/:id', loadComponent: () => import('./proveedores/proveedor-detail.component').then(m => m.ProveedorDetailComponent) },
  { path: 'ordenes', loadComponent: () => import('./ordenes/oc-list.component').then(m => m.OcListComponent) },
  { path: 'ordenes/:id', loadComponent: () => import('./ordenes/oc-detail.component').then(m => m.OCDetailComponent) },
  { path: 'recepciones', loadComponent: () => import('./recepciones/recepciones-list.component').then(m => m.RecepcionesListComponent) },
];
