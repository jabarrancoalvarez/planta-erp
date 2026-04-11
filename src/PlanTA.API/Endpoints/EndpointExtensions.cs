using System.Reflection;

namespace PlanTA.API.Endpoints;

public static class EndpointExtensions
{
    public static WebApplication MapEndpointGroups(this WebApplication app)
    {
        var endpointGroupTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IEndpointGroup).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });

        foreach (var type in endpointGroupTypes)
        {
            var instance = (IEndpointGroup)Activator.CreateInstance(type)!;
            var prefix = instance.RoutePrefix ?? InferRoutePrefix(type.Name);
            var group = app.MapGroup(prefix);
            instance.MapEndpoints(group);
        }

        return app;
    }

    private static string InferRoutePrefix(string typeName)
    {
        // ProductosEndpoints → /api/v1/productos
        var name = typeName.Replace("Endpoints", "", StringComparison.OrdinalIgnoreCase);
        var kebab = ToKebabCase(name);
        return $"/api/v1/{kebab}";
    }

    private static string ToKebabCase(string input)
    {
        return string.Concat(
            input.Select((c, i) =>
                i > 0 && char.IsUpper(c)
                    ? $"-{char.ToLowerInvariant(c)}"
                    : $"{char.ToLowerInvariant(c)}"));
    }
}
