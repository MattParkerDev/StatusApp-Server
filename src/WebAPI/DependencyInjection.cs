using Application;
using Domain;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebAPI.SignalR;

namespace WebAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApi(
        this IServiceCollection services,
        IConfiguration configuration,
        string corsPolicyName
    )
    {
        var dbConnectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<IStatusContext, StatusContext>(
            options => options.UseNpgsql(dbConnectionString).EnableSensitiveDataLogging()
        );
        services
            .AddIdentityCore<User>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
            .AddEntityFrameworkStores<StatusContext>()
            .AddSignInManager<SignInManager<User>>();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
            })
            .AddIdentityCookies();

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.SameSite = SameSiteMode.None;
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.Clear();
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
        });
        services.AddAuthorization();
        services.AddSignalR();

        services.AddOpenApiDocument(configure =>
        {
            configure.Title = "StatusApp Api";
        });

        services.AddEndpointsApiExplorer();

        services.AddCors(
            options =>
                options.AddPolicy(
                    name: corsPolicyName,
                    policy =>
                        policy
                            .WithOrigins(
                                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                                == "Development"
                                    ? "https://localhost:5001"
                                    : "https://red-ground-0805be400.2.azurestaticapps.net"
                            )
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials()
                )
        );

        services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>();

        return services;
    }
}
