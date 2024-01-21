using Microsoft.EntityFrameworkCore;
using Application;
using Application.Services;
using Infrastructure;
using Infrastructure.Persistence;
using WebAPI;
using WebAPI.Routes;
using WebAPI.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

const string StatusAppCorsPolicy = nameof(StatusAppCorsPolicy);
builder.Services.AddWebApi(builder.Configuration, StatusAppCorsPolicy);

var app = builder.Build();

// CORS
app.UseCors(StatusAppCorsPolicy);

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
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

await db.SignalRConnections.ExecuteDeleteAsync();

await testDataGeneratorService.SeedTestData();

app.MapAuthRoutes();
app.MapFriendRoutes();
app.MapMessageRoutes();
app.MapUserRoutes();

app.MapHub<StatusHub>("/statushub");

await app.RunAsync();
