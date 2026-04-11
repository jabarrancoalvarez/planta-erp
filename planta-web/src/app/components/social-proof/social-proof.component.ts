import { Component } from '@angular/core';
import { ScrollAnimateDirective } from '../../shared/directives/scroll-animate.directive';

interface Testimonial {
  quote: string;
  name: string;
  role: string;
  company: string;
  initials: string;
  color: string;
}

interface Logo {
  name: string;
}

@Component({
  selector: 'app-social-proof',
  standalone: true,
  imports: [ScrollAnimateDirective],
  templateUrl: './social-proof.component.html',
  styleUrl: './social-proof.component.scss',
})
export class SocialProofComponent {
  readonly stars = [1, 2, 3, 4, 5];

  readonly logos: Logo[] = [
    { name: 'MetalParts S.L.' },
    { name: 'IndustrialTech' },
    { name: 'AgroMaq' },
    { name: 'TechnoPlant' },
    { name: 'MecaGroup' },
    { name: 'Industrias Norte' },
  ];

  readonly testimonials: Testimonial[] = [
    {
      quote: 'Pasamos de perder incidencias a resolverlas en horas. FixFlow es absolutamente indispensable para nuestro equipo.',
      name: 'Carlos M.',
      role: 'Jefe de Mantenimiento',
      company: 'MetalParts S.L.',
      initials: 'CM',
      color: '#2563eb',
    },
    {
      quote: 'La planificación automática nos ahorró más de 6 horas semanales. El equipo lo adoptó sin ningún problema.',
      name: 'Ana R.',
      role: 'Directora de Operaciones',
      company: 'AgroMaq',
      initials: 'AR',
      color: '#10b981',
    },
    {
      quote: 'Por fin tenemos visibilidad total de nuestros 80 equipos. Antes teníamos todo en Excel y era un caos absoluto.',
      name: 'Miguel T.',
      role: 'Gerente General',
      company: 'IndustrialTech',
      initials: 'MT',
      color: '#0ea5e9',
    },
  ];
}
