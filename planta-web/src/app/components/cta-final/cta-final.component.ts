import { Component } from '@angular/core';
import { ScrollAnimateDirective } from '../../shared/directives/scroll-animate.directive';

@Component({
  selector: 'app-cta-final',
  standalone: true,
  imports: [ScrollAnimateDirective],
  templateUrl: './cta-final.component.html',
  styleUrl: './cta-final.component.scss',
})
export class CtaFinalComponent {
  onRequestDemo(): void {
    console.log('CTA final: Solicitar demo gratuita');
  }

  onViewPricing(): void {
    const el = document.getElementById('pricing');
    if (el) {
      el.scrollIntoView({ behavior: 'smooth' });
    }
  }
}
