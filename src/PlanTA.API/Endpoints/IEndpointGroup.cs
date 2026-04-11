namespace PlanTA.API.Endpoints;

public interface IEndpointGroup
{
    void MapEndpoints(RouteGroupBuilder group);

    /// <summary>Override para personalizar la ruta base. Si es null, se infiere del nombre de la clase.</summary>
    string? RoutePrefix => null;
}
