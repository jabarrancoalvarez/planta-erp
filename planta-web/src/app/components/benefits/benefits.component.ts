import { Component } from '@angular/core';
import { ScrollAnimateDirective } from '../../shared/directives/scroll-animate.directive';

interface Benefit {
  icon: string;
  stat: string;
  title: string;
  description: string;
}

@Component({
  selector: 'app-benefits',
  standalone: true,
  imports: [ScrollAnimateDirective],
  templateUrl: './benefits.component.html',
  styleUrl: './benefits.component.scss',
})
export class BenefitsComponent {
  readonly benefits: Benefit[] = [
    {
      icon: '⏱️',
      stat: '−6h',
      title: 'Ahorra tiempo',
      description: 'Elimina llamadas, WhatsApps y hojas de cálculo. Todo en un solo lugar.',
    },
    {
      icon: '📉',
      stat: '−40%',
      title: 'Reduce errores',
      description: 'Sin incidencias perdidas ni mantenimientos olvidados. Control total.',
    },
    {
      icon: '📊',
      stat: '100%',
      title: 'Control total',
      description: 'Dashboards en tiempo real con el estado de todos tus activos.',
    },
    {
      icon: '🔧',
      stat: '+35%',
      title: 'Mejora el servicio',
      description: 'Técnicos más eficientes, clientes más satisfechos y menos paradas.',
    },
  ];
}
