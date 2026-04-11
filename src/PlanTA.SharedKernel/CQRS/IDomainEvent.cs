using MediatR;

namespace PlanTA.SharedKernel.CQRS;

/// <summary>Evento de dominio publicado via MediatR.Publish.</summary>
public interface IDomainEvent : INotification;
