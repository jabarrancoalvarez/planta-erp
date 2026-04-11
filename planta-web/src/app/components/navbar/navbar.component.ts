import { Component, HostListener, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss',
})
export class NavbarComponent implements OnInit {
  isScrolled = signal(false);
  isMobileMenuOpen = signal(false);

  ngOnInit(): void {
    this.checkScroll();
  }

  @HostListener('window:scroll', [])
  onScroll(): void {
    this.checkScroll();
  }

  private checkScroll(): void {
    this.isScrolled.set(window.scrollY > 20);
  }

  toggleMobileMenu(): void {
    this.isMobileMenuOpen.update((v) => !v);
  }

  closeMobileMenu(): void {
    this.isMobileMenuOpen.set(false);
  }

  scrollTo(sectionId: string): void {
    this.closeMobileMenu();
    const el = document.getElementById(sectionId);
    if (el) {
      el.scrollIntoView({ behavior: 'smooth' });
    }
  }

  requestDemo(): void {
    console.log('CTA navbar: Solicitar demo');
  }
}
