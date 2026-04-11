import { Routes } from '@angular/router';

export const VENTAS_ROUTES: Routes = [
  { path: '', redirectTo: 'pedidos', pathMatch: 'full' },
  { path: 'clientes', loadComponent: () => import('./clientes/clientes-list.component').then(m => m.ClientesListComponent) },
  { path: 'clientes/:id', loadComponent: () => import('./clientes/cliente-detail.component').then(m => m.ClienteDetailComponent) },
  { path: 'pedidos', loadComponent: () => import('./pedidos/pedidos-list.component').then(m => m.PedidosListComponent) },
  { path: 'pedidos/:id', loadComponent: () => import('./pedidos/pedido-detail.component').then(m => m.PedidoDetailComponent) },
  { path: 'expediciones', loadComponent: () => import('./expediciones/expediciones-list.component').then(m => m.ExpedicionesListComponent) },
];
