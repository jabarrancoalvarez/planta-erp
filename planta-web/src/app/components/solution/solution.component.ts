import { Component } from '@angular/core';
import { ScrollAnimateDirective } from '../../shared/directives/scroll-animate.directive';

interface Feature {
  icon: string;
  title: string;
  description: string;
}

@Component({
  selector: 'app-solution',
  standalone: true,
  imports: [ScrollAnimateDirective],
  templateUrl: './solution.component.html',
  styleUrl: './solution.component.scss',
})
export class SolutionComponent {
  readonly features: Feature[] = [
    {
      icon: '🏭',
      title: 'Gestión de activos',
      description: 'Registra y controla todas tus máquinas y equipos con su historial completo.',
    },
    {
      icon: '📅',
      title: 'Planificación de mantenimiento',
      description: 'Programa revisiones con alertas automáticas antes de que algo falle.',
    },
    {
      icon: '🚨',
      title: 'Gestión de incidencias',
      description: 'Abre, asigna y cierra incidencias en segundos desde cualquier dispositivo.',
    },
    {
      icon: '📱',
      title: 'App para técnicos',
      description: 'Tus técnicos reportan desde el móvil en tiempo real, sin papel.',
    },
    {
      icon: '📋',
      title: 'Historial completo',
      description: 'Accede al historial de cada máquina al instante, en cualquier momento.',
    },
    {
      icon: '🔔',
      title: 'Alertas automáticas',
      description: 'Recibe notificaciones por email y push antes de que algo falle.',
    },
  ];
}
