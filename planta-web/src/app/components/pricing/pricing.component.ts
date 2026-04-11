import { Component, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ScrollAnimateDirective } from '../../shared/directives/scroll-animate.directive';

interface PricingPlan {
  name: string;
  monthlyPrice: number;
  description: string;
  features: string[];
  cta: string;
  highlighted: boolean;
  badge?: string;
}

@Component({
  selector: 'app-pricing',
  standalone: true,
  imports: [CommonModule, ScrollAnimateDirective],
  templateUrl: './pricing.component.html',
  styleUrl: './pricing.component.scss',
})
export class PricingComponent {
  isAnnual = signal(false);

  readonly plans: PricingPlan[] = [
    {
      name: 'Básico',
      monthlyPrice: 29,
      description: 'Perfecto para pequeñas empresas que quieren empezar.',
      features: [
        'Hasta 20 activos',
        '2 usuarios incluidos',
        'Incidencias ilimitadas',
        'App para técnicos',
        'Soporte por email',
        'Historial 6 meses',
      ],
      cta: 'Empezar gratis',
      highlighted: false,
    },
    {
      name: 'Profesional',
      monthlyPrice: 79,
      description: 'Para equipos que necesitan más potencia y control.',
      features: [
        'Activos ilimitados',
        '10 usuarios incluidos',
        'Incidencias ilimitadas',
        'App para técnicos',
        'Soporte prioritario',
        'Historial completo',
        'Informes avanzados',
        'Integraciones API',
      ],
      cta: 'Solicitar demo',
      highlighted: true,
      badge: 'Más popular',
    },
    {
      name: 'Enterprise',
      monthlyPrice: 199,
      description: 'Para grandes organizaciones con requisitos avanzados.',
      features: [
        'Activos ilimitados',
        'Usuarios ilimitados',
        'Incidencias ilimitadas',
        'App para técnicos',
        'Soporte 24/7 dedicado',
        'Historial completo',
        'Informes a medida',
        'API + webhooks',
        'SLA garantizado',
        'Formación incluida',
      ],
      cta: 'Contactar ventas',
      highlighted: false,
    },
  ];

  getPrice(plan: PricingPlan): number {
    if (this.isAnnual()) {
      return Math.round(plan.monthlyPrice * 0.8);
    }
    return plan.monthlyPrice;
  }

  toggleBilling(): void {
    this.isAnnual.update((v) => !v);
  }

  onPlanCta(plan: PricingPlan): void {
    console.log(`CTA pricing: ${plan.cta} — Plan ${plan.name}`);
  }
}
