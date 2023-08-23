using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Application;
using Application.Contracts;
using Domain;
using Infrastructure;
using WebAPI.Routes;
using WebAPI.SignalR;

var builder = WebApplication.CreateBuilder(args);

// ReSharper disable once RedundantAssignment
var connectionString = string.Empty;
if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    connectionString = builder.Configuration.GetConnectionString("LocalConnection");
}
else
{
    connectionString = Environment.GetEnvironmentVariable("ConnectionString");
}

builder.Services.AddDbContext<IStatusContext, StatusContext>(
    options => options.UseNpgsql(connectionString).EnableSensitiveDataLogging()
);
builder.Services
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

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
        options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
    })
    .AddIdentityCookies();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.Clear();
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
});
builder.Services.AddAuthorization();
builder.Services.AddSignalR();

builder.Services.AddSingleton<IUserIdProvider, SignalRUserIdProvider>();
builder.Services.AddScoped<IMessagingService, MessagingService>();
builder.Services.AddScoped<IFriendshipService, FriendshipService>();
builder.Services.AddScoped<IIdentityUserService, IdentityUserService>();
builder.Services.AddScoped<IStatusUserService, StatusUserService>();
builder.Services.AddScoped<IStatusUserService, StatusUserService>();
builder.Services.AddScoped<TestDataGeneratorService>();

builder.Services.AddOpenApiDocument(configure =>
{
    configure.Title = "StatusApp Api";
});

builder.Services.AddEndpointsApiExplorer();

const string StatusAppCorsPolicy = nameof(StatusAppCorsPolicy);
builder.Services.AddCors(
    options =>
        options.AddPolicy(
            name: StatusAppCorsPolicy,
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

var app = builder.Build();

// CORS
app.UseCors(StatusAppCorsPolicy);

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi3();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

using var scope = app.Services.CreateScope();
await using var db = scope.ServiceProvider.GetRequiredService<StatusContext>();
var testDataGeneratorService = scope.ServiceProvider.GetRequiredService<TestDataGeneratorService>();

//Only during development
await db.Database.EnsureDeletedAsync();
await db.Database.EnsureCreatedAsync();

await db.Connections.ExecuteDeleteAsync();

await testDataGeneratorService.SeedTestData();

app.MapAuthRoutes();
app.MapFriendRoutes();
app.MapMessageRoutes();
app.MapUserRoutes();

app.MapHub<StatusHub>("/statushub");

app.Run();
