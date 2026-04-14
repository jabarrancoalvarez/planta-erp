using PlanTA.Importador.Domain.Enums;
using PlanTA.SharedKernel.CQRS;

namespace PlanTA.Importador.Application.Features.Jobs.CreateJob;

public record CreateImportJobCommand(
    TipoImportacion Tipo,
    FormatoArchivo Formato,
    string NombreArchivo,
    Guid UserId) : ICommand<Guid>;
