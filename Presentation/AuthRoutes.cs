namespace StatusApp_Server.Presentation;

public static class AuthRoutes
{
    public static void MapAuthRoutes(this WebApplication app)
    {
        app.MapGet(
                "/checkAuth",
                (HttpContext context) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    return userName;
                }
            )
            .RequireAuthorization()
            .WithName("CheckAuth")
            .WithOpenApi();
    }
}
