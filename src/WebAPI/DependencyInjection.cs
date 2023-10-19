using Application;
using Domain.Entities;
using Infrastructure.Persistence;
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

        var allowedOrigins = configuration.GetRequiredSection("AllowedOrigins").Get<string[]>();

        services.AddCors(
            options =>
                options.AddPolicy(
                    name: corsPolicyName,
                    policy =>
                        policy
                            .WithOrigins(allowedOrigins)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials()
                )
        );

        services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>();

        return services;
    }
}
