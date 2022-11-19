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
    FirstName = "Maurice",
    LastName = "Smith",
    UserName = "MauriceSmith",
    Status = "Open to plans",
    Online = true
};

var newAccount = new Account
{
    Email = "maurice@email.com",
    UserName = "MauriceSmith",
    Password = "password",
    PhoneNumber = "0418354655",
};

var secondUser = new User
{
    FirstName = "Katie",
    LastName = "Murray",
    UserName = "Katieee",
    Status = "Keen for dinner",
    Online = true
};

var secondAccount = new Account
{
    Email = "katie@email.com",
    UserName = "Katieee",
    Password = "password",
    PhoneNumber = "0432485567",
};

var thirdUser = new User
{
    FirstName = "Jarrod",
    LastName = "Lee",
    UserName = "Jrod1",
    Status = "Quiet night in",
    Online = false
};

var thirdAccount = new Account
{
    Email = "jrod@email.com",
    UserName = "Jrod1",
    Password = "password",
    PhoneNumber = "0418213324",
};

var newFriendship = new Friendship
{
    AccountId = 1,
    FriendId = 2,
    Accepted = true,
    AreFriends = true,
    BecameFriendsDate = DateTime.UtcNow
};

var secondFriendship = new Friendship
{
    AccountId = 2,
    FriendId = 1,
    Accepted = true,
    AreFriends = true,
    BecameFriendsDate = DateTime.UtcNow
};

db.Users.Add(newUser);
db.Users.Add(secondUser);
db.Users.Add(thirdUser);
db.Accounts.Add(newAccount);
db.Accounts.Add(secondAccount);
db.Accounts.Add(thirdAccount);
db.Friendships.Add(newFriendship);
db.Friendships.Add(secondFriendship);
db.Messages.Add(new() { Data = "Test", ChatId = 1 });
db.Messages.Add(newMessage);
await db.SaveChangesAsync();

app.RegisterMessageAPIs();
app.RegisterUserAPIs();
app.RegisterFriendAPIs();

app.Run();
