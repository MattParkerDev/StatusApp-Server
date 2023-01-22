using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
    .AddEntityFrameworkStores<ChatContext>()
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
builder.Services.AddScoped<FriendshipService>();
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
using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

//Only during development
await db.Database.EnsureDeletedAsync();
await db.Database.EnsureCreatedAsync();

db.Connections.ExecuteDelete();

var guid1 = Guid.NewGuid();
var friendship1 = new Friendship
{
    UserName = "BigMaurice",
    FriendUserName = "Katie11",
    Accepted = true,
    AreFriends = true,
    BecameFriendsDate = DateTime.UtcNow,
    FriendFirstName = "Katie",
    FriendLastName = "Murray",
    GroupId = guid1
};

var friendship2 = new Friendship
{
    UserName = "Katie11",
    FriendUserName = "BigMaurice",
    Accepted = true,
    AreFriends = true,
    BecameFriendsDate = DateTime.UtcNow,
    FriendFirstName = "Maurice",
    FriendLastName = "Smith",
    GroupId = guid1
};

var guid2 = Guid.NewGuid();
var friendship3 = new Friendship
{
    UserName = "BigMaurice",
    FriendUserName = "Jrod1",
    Accepted = true,
    AreFriends = true,
    BecameFriendsDate = DateTime.UtcNow,
    FriendFirstName = "Jarrod",
    FriendLastName = "Lee",
    GroupId = guid2
};

var friendship4 = new Friendship
{
    UserName = "Jrod1",
    FriendUserName = "BigMaurice",
    Accepted = true,
    AreFriends = true,
    BecameFriendsDate = DateTime.UtcNow,
    FriendFirstName = "Maurice",
    FriendLastName = "Smith",
    GroupId = guid2
};

var newIdentityUser = new User
{
    UserName = "BigMaurice",
    Email = "Bigmaurice@gmail.com",
    FirstName = "Maurice",
    LastName = "Smith",
    Status = "Open to Plans",
    Online = true,
};
var newIdentityUser2 = new User
{
    UserName = "Katie11",
    Email = "katie@hotmail.com",
    FirstName = "Katie",
    LastName = "Murray",
    Status = "Keen for dinner",
    Online = true,
};
var newIdentityUser3 = new User
{
    UserName = "Jrod1",
    Email = "jrod1@hotmail.com",
    FirstName = "Jarrod",
    LastName = "Lee",
    Status = "Quiet night in",
    Online = false,
};

await userManager.CreateAsync(newIdentityUser, "password1");
await userManager.CreateAsync(newIdentityUser2, "password1");
await userManager.CreateAsync(newIdentityUser3, "password1");

db.Friendships.Add(friendship1);
db.Friendships.Add(friendship2);
db.Friendships.Add(friendship3);
db.Friendships.Add(friendship4);

await db.SaveChangesAsync();

app.RegisterMessageAPIs();
app.RegisterUserAPIs();
app.RegisterFriendAPIs();
app.RegisterAuthAPIs();
app.MapHub<StatusHub>("/statushub");

app.Run();
