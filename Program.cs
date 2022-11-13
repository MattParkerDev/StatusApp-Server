using Microsoft.EntityFrameworkCore;
using StatusApp_Server.Application;
using StatusApp_Server.Domain;
using StatusApp_Server.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

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

//await db.Database.EnsureDeletedAsync();
await db.Database.EnsureCreatedAsync();

var newMessage = new Message();
newMessage.Data = "test2";
newMessage.ChatId = 2;

var newUser = new User();
newUser.FirstName = "Umang";
newUser.LastName = "Smith";
newUser.Email = "test@email.com";
newUser.UserName = "GoatRat";

db.Users.Add(newUser);
db.Messages.Add(new() { Data = "Test", ChatId = 1 });
db.Messages.Add(newMessage);
await db.SaveChangesAsync();

app.RegisterMessageAPIs();
app.RegisterUserAPIs();

app.Run();
