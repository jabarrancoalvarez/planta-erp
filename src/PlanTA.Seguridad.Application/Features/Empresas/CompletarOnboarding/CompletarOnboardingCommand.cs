using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Seguridad.Application.Features.Empresas.CompletarOnboarding;

public record CompletarOnboardingCommand : ICommand<bool>;
