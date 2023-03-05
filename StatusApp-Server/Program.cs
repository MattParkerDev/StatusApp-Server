using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StatusApp_Server.Application;
using StatusApp_Server.Application.Contracts;
using StatusApp_Server.Domain;
using StatusApp_Server.Infrastructure;
using StatusApp_Server.Presentation;

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

builder.Services.AddDbContext<StatusContext>(
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
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<TestDataGeneratorService>();

builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerDocument();

builder.Services.AddCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // CORS
    app.UseCors(
        builder =>
            builder
                .WithOrigins("https://localhost:5001")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
    );
}

// Configure the HTTP request pipeline.
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

await testDataGeneratorService.GenerateTestUsersAndFriendships();

app.MapAuthRoutes();
app.MapFriendRoutes();
app.MapMessageRoutes();
app.MapUserRoutes();

app.MapHub<StatusHub>("/statushub");

app.Run();
