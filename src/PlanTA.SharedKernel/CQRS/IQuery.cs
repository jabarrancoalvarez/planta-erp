using MediatR;

namespace PlanTA.SharedKernel.CQRS;

/// <summary>Query de solo lectura. No abre transacción.</summary>
public interface IQuery<T> : IRequest<Result<T>>;
