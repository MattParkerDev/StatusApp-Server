using StatusApp.Server.Application.Contracts;
using StatusApp.Server.Domain;

namespace StatusApp.Server.Application;

public class TestDataGeneratorService
{
    private readonly IIdentityUserService _identityUserService;
    private readonly IFriendshipService _friendshipService;

    public TestDataGeneratorService(IIdentityUserService identityUserService, IFriendshipService friendshipService)
    {
        _identityUserService = identityUserService;
        _friendshipService = friendshipService;
    }

    public async Task GenerateTestUsersAndFriendships()
    {
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

        await _identityUserService.CreateUserAsync(newIdentityUser, "password1");
        await _identityUserService.CreateUserAsync(newIdentityUser2, "password1");
        await _identityUserService.CreateUserAsync(newIdentityUser3, "password1");

        await _friendshipService.CreateAcceptedFriendshipPair(newIdentityUser, newIdentityUser2);
        await _friendshipService.CreateAcceptedFriendshipPair(newIdentityUser, newIdentityUser3);
    }
}
