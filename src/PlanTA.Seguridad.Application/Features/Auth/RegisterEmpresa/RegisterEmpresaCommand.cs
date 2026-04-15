using PlanTA.SharedKernel.CQRS;
using PlanTA.Seguridad.Application.DTOs;

namespace PlanTA.Seguridad.Application.Features.Auth.RegisterEmpresa;

public record RegisterEmpresaCommand(
    string NombreEmpresa,
    string? Cif,
    string EmailAdmin,
    string Password,
    string NombreAdmin) : ICommand<TokenPairDto>;
