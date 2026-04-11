import { Component } from '@angular/core';
import { NavbarComponent } from '../../components/navbar/navbar.component';
import { HeroComponent } from '../../components/hero/hero.component';
import { ProblemComponent } from '../../components/problem/problem.component';
import { SolutionComponent } from '../../components/solution/solution.component';
import { BenefitsComponent } from '../../components/benefits/benefits.component';
import { HowItWorksComponent } from '../../components/how-it-works/how-it-works.component';
import { SocialProofComponent } from '../../components/social-proof/social-proof.component';
import { PricingComponent } from '../../components/pricing/pricing.component';
import { CtaFinalComponent } from '../../components/cta-final/cta-final.component';
import { FooterComponent } from '../../components/footer/footer.component';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [
    NavbarComponent,
    HeroComponent,
    ProblemComponent,
    SolutionComponent,
    BenefitsComponent,
    HowItWorksComponent,
    SocialProofComponent,
    PricingComponent,
    CtaFinalComponent,
    FooterComponent,
  ],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.scss',
})
export class LandingComponent {}
