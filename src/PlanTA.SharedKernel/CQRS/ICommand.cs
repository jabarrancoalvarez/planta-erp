using MediatR;

namespace PlanTA.SharedKernel.CQRS;

/// <summary>Comando que modifica estado. Se ejecuta dentro de una transacción.</summary>
public interface ICommand<T> : IRequest<Result<T>>;
