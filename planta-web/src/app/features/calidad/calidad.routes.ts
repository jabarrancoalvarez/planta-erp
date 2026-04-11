import { Routes } from '@angular/router';

export const CALIDAD_ROUTES: Routes = [
  { path: '', redirectTo: 'inspecciones', pathMatch: 'full' },
  { path: 'plantillas', loadComponent: () => import('./plantillas/plantillas-list.component').then(m => m.PlantillasListComponent) },
  { path: 'plantillas/:id', loadComponent: () => import('./plantillas/plantilla-detail.component').then(m => m.PlantillaDetailComponent) },
  { path: 'inspecciones', loadComponent: () => import('./inspecciones/inspecciones-list.component').then(m => m.InspeccionesListComponent) },
  { path: 'no-conformidades', loadComponent: () => import('./no-conformidades/nc-list.component').then(m => m.NcListComponent) },
  { path: 'no-conformidades/:id', loadComponent: () => import('./no-conformidades/nc-detail.component').then(m => m.NCDetailComponent) },
];
