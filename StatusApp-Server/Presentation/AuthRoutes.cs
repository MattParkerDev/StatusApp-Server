using Microsoft.AspNetCore.Http.HttpResults;

namespace StatusApp_Server.Presentation;

public static class AuthRoutes
{
    public static void MapAuthRoutes(this WebApplication app)
    {
        app.MapGet(
                "/checkAuth",
                Ok<string> (HttpContext context) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    return TypedResults.Ok(userName);
                }
            )
            .RequireAuthorization()
            .WithName("CheckAuth")
            .WithOpenApi();
    }
}
