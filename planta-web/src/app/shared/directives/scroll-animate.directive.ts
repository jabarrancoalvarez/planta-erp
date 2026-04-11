import {
  Directive,
  ElementRef,
  OnInit,
  OnDestroy,
  Input,
  Renderer2,
} from '@angular/core';

@Directive({
  selector: '[scrollAnimate]',
  standalone: true,
})
export class ScrollAnimateDirective implements OnInit, OnDestroy {
  @Input() scrollAnimateDelay = 0;
  @Input() scrollAnimateThreshold = 0.15;

  private observer: IntersectionObserver | null = null;

  constructor(private el: ElementRef, private renderer: Renderer2) {}

  ngOnInit(): void {
    const element = this.el.nativeElement as HTMLElement;
    this.renderer.addClass(element, 'scroll-animate');

    if (this.scrollAnimateDelay > 0) {
      this.renderer.setStyle(
        element,
        'transition-delay',
        `${this.scrollAnimateDelay}ms`
      );
    }

    this.observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            this.renderer.addClass(element, 'visible');
            this.observer?.unobserve(element);
          }
        });
      },
      { threshold: this.scrollAnimateThreshold, rootMargin: '0px 0px -40px 0px' }
    );

    this.observer.observe(element);
  }

  ngOnDestroy(): void {
    this.observer?.disconnect();
  }
}
