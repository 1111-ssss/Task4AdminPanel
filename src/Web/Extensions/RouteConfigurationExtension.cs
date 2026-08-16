using Web.Endpoints;

namespace Web.Extensions;

public static class RoutingExtensions
{
    public static void MapCustomRoutes(this WebApplication app)
    {
        app.MapAccountEndpoints();
    }
}