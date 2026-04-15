import { Component, inject, signal, computed } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';

interface NavItem {
  label: string;
  icon: string;
  route: string;
  badge?: number;
  roles?: string[];
}

const ALL_ROLES = ['Administrador', 'GerentePlanta', 'JefeAlmacen', 'JefeProduccion', 'Compras', 'Ventas', 'Operario', 'Calidad'];

@Component({
  selector: 'app-app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './app-shell.component.html',
  styleUrl: './app-shell.component.scss',
})
export class AppShellComponent {
  readonly auth = inject(AuthService);

  sidebarCollapsed = signal(false);

  firstName(): string {
    return this.auth.currentUser()?.name?.split(' ')[0] ?? '';
  }

  readonly trialInfo = computed(() => {
    const u = this.auth.currentUser();
    if (!u?.trialHasta) return null;
    const hasta = new Date(u.trialHasta);
    const dias = Math.ceil((hasta.getTime() - Date.now()) / (1000 * 60 * 60 * 24));
    if (dias < 0) return { expirado: true, dias: 0, texto: 'Trial expirado — suscríbete para seguir usando PlanTA' };
    if (dias === 0) return { expirado: false, dias, texto: 'Último día de trial' };
    return { expirado: false, dias, texto: `Trial: ${dias} día${dias === 1 ? '' : 's'} restante${dias === 1 ? '' : 's'}` };
  });
  mobileSidebarOpen = signal(false);
  userMenuOpen = signal(false);

  private readonly allNavItems: NavItem[] = [
    { label: 'Dashboard', icon: 'grid', route: '/app/dashboard', roles: ALL_ROLES },
    { label: 'Inventario', icon: 'package', route: '/app/inventario', roles: ['Administrador', 'GerentePlanta', 'JefeAlmacen', 'JefeProduccion', 'Compras', 'Operario', 'Calidad'] },
    { label: 'Produccion', icon: 'factory', route: '/app/produccion', roles: ['Administrador', 'GerentePlanta', 'JefeProduccion', 'Operario'] },
    { label: 'Compras', icon: 'shopping-cart', route: '/app/compras', roles: ['Administrador', 'GerentePlanta', 'Compras'] },
    { label: 'Ventas', icon: 'truck', route: '/app/ventas', roles: ['Administrador', 'GerentePlanta', 'Ventas'] },
    { label: 'Facturacion', icon: 'file-text', route: '/app/facturacion', roles: ['Administrador', 'GerentePlanta', 'Ventas'] },
    { label: 'Calidad', icon: 'check-circle', route: '/app/calidad', roles: ['Administrador', 'GerentePlanta', 'Calidad'] },
    { label: 'Activos', icon: 'cpu', route: '/app/activos', roles: ['Administrador', 'GerentePlanta', 'JefeProduccion'] },
    { label: 'Mantenimiento', icon: 'tool', route: '/app/mantenimiento', roles: ['Administrador', 'GerentePlanta', 'JefeProduccion', 'Operario'] },
    { label: 'Incidencias', icon: 'alert-triangle', route: '/app/incidencias', roles: ALL_ROLES },
    { label: 'CRM', icon: 'users', route: '/app/crm', roles: ['Administrador', 'GerentePlanta', 'Ventas'] },
    { label: 'RRHH', icon: 'user', route: '/app/rrhh', roles: ['Administrador', 'GerentePlanta'] },
    { label: 'Costes', icon: 'dollar-sign', route: '/app/costes', roles: ['Administrador', 'GerentePlanta'] },
    { label: 'OEE', icon: 'activity', route: '/app/oee', roles: ['Administrador', 'GerentePlanta', 'JefeProduccion'] },
    { label: 'Importador', icon: 'upload', route: '/app/importador', roles: ['Administrador'] },
    { label: 'Asistente IA', icon: 'message-square', route: '/app/ia', roles: ALL_ROLES },
    { label: 'Movil', icon: 'smartphone', route: '/app/movil', roles: ['Administrador', 'Operario'] },
    { label: 'Configuracion', icon: 'settings', route: '/app/settings', roles: ALL_ROLES },
  ];

  readonly navItems = computed(() => {
    const role = this.auth.currentUser()?.role;
    if (!role) return [];
    return this.allNavItems.filter(item => !item.roles || item.roles.includes(role));
  });

  toggleSidebar(): void {
    this.sidebarCollapsed.update(v => !v);
  }

  toggleMobileSidebar(): void {
    this.mobileSidebarOpen.update(v => !v);
  }

  closeMobileSidebar(): void {
    this.mobileSidebarOpen.set(false);
  }

  toggleUserMenu(): void {
    this.userMenuOpen.update(v => !v);
  }

  logout(): void {
    this.auth.logout();
  }
}
