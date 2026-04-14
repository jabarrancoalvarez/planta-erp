import { Routes } from '@angular/router';

export const IA_ROUTES: Routes = [
  { path: '', redirectTo: 'chat', pathMatch: 'full' },
  {
    path: 'chat',
    loadComponent: () => import('./ia-chat.component').then(m => m.IaChatComponent),
  },
];
