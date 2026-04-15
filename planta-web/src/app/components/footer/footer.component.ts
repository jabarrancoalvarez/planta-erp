import { Component } from '@angular/core';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss',
})
export class FooterComponent {
  readonly year = new Date().getFullYear();

  readonly links = {
    producto: [
      { label: 'Características', href: '#solution' },
      { label: 'Precios', href: '#pricing' },
      { label: 'Cómo funciona', href: '#how-it-works' },
      { label: 'Integraciones', href: '#' },
    ],
    empresa: [
      { label: 'Sobre PlanTA', href: '#' },
      { label: 'Blog', href: '#' },
      { label: 'Casos de éxito', href: '#social-proof' },
      { label: 'Contacto', href: '#' },
    ],
    legal: [
      { label: 'Privacidad', href: '#' },
      { label: 'Términos de uso', href: '#' },
      { label: 'Cookies', href: '#' },
      { label: 'RGPD', href: '#' },
    ],
  };

  scrollTo(sectionId: string): void {
    const el = document.getElementById(sectionId.replace('#', ''));
    if (el) {
      el.scrollIntoView({ behavior: 'smooth' });
    }
  }
}
