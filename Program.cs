using Microsoft.EntityFrameworkCore;
using StatusApp_Server.Application;
using StatusApp_Server.Domain;
using StatusApp_Server.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var connectionString = "";
if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
{
    connectionString = builder.Configuration.GetConnectionString("LocalConnection");
}
else
{
    connectionString = Environment.GetEnvironmentVariable("ConnectionString");
}

builder.Services.AddDbContext<ChatContext>(
    options => options.UseNpgsql(connectionString).EnableSensitiveDataLogging()
);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

using var scope = app.Services.CreateScope();
await using var db = scope.ServiceProvider.GetRequiredService<ChatContext>();

//Only during development
await db.Database.EnsureDeletedAsync();
await db.Database.EnsureCreatedAsync();

var newMessage = new Message { Data = "test2", ChatId = 2 };

var newUser = new User
{
    FirstName = "Umang",
    LastName = "Smith",
    Email = "test@email.com",
    UserName = "GoatRat",
    Password = "password",
    PhoneNumber = "0418354655",
    Status = "Available"
};

db.Users.Add(newUser);
db.Messages.Add(new() { Data = "Test", ChatId = 1 });
db.Messages.Add(newMessage);
await db.SaveChangesAsync();

app.RegisterMessageAPIs();
app.RegisterUserAPIs();

app.Run();
