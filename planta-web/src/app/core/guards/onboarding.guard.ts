import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const onboardingGuard: CanActivateFn = (route) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const user = auth.currentUser();

  if (!user) return true;
  if (user.onboardingCompletado) return true;

  if (route.routeConfig?.path === 'onboarding') return true;

  return router.createUrlTree(['/app/onboarding']);
};
