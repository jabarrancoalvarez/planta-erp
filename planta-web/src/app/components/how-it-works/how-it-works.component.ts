import { Component } from '@angular/core';
import { ScrollAnimateDirective } from '../../shared/directives/scroll-animate.directive';

interface Step {
  number: string;
  icon: string;
  title: string;
  description: string;
}

@Component({
  selector: 'app-how-it-works',
  standalone: true,
  imports: [ScrollAnimateDirective],
  templateUrl: './how-it-works.component.html',
  styleUrl: './how-it-works.component.scss',
})
export class HowItWorksComponent {
  readonly steps: Step[] = [
    {
      number: '01',
      icon: '🏭',
      title: 'Registra tus equipos',
      description: 'Importa o crea manualmente el listado de máquinas y activos de tu empresa. Añade fichas técnicas, fotos y documentación.',
    },
    {
      number: '02',
      icon: '📅',
      title: 'Planifica el mantenimiento',
      description: 'Define frecuencias, asigna técnicos responsables y configura alertas automáticas. El sistema te avisa antes de que venza.',
    },
    {
      number: '03',
      icon: '⚡',
      title: 'Gestiona en tiempo real',
      description: 'Recibe incidencias, asígnalas y ciérralas desde cualquier dispositivo. Tus técnicos reportan desde el móvil al instante.',
    },
  ];
}
