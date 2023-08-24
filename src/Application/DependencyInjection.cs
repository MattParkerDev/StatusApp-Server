using Application.Services;
using Application.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddScoped<IMessagingService, MessagingService>();
        services.AddScoped<IFriendshipService, FriendshipService>();
        services.AddScoped<IIdentityUserService, IdentityUserService>();
        services.AddScoped<IStatusUserService, StatusUserService>();
        services.AddScoped<TestDataGeneratorService>();

        return services;
    }
}
