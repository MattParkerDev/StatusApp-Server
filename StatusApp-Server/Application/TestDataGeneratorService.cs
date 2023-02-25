using Microsoft.AspNetCore.Identity;
using StatusApp_Server.Domain;
using StatusApp_Server.Infrastructure;

namespace StatusApp_Server.Application;

public class TestDataGeneratorService
{
    private readonly UserService _userService;
    private readonly FriendshipService _friendshipService;

    public TestDataGeneratorService(UserService userService, FriendshipService friendshipService)
    {
        _userService = userService;
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

        await _userService.CreateUserAsync(newIdentityUser, "password1");
        await _userService.CreateUserAsync(newIdentityUser2, "password1");
        await _userService.CreateUserAsync(newIdentityUser3, "password1");

        await _friendshipService.CreateAcceptedFriendshipPair(newIdentityUser, newIdentityUser2);
        await _friendshipService.CreateAcceptedFriendshipPair(newIdentityUser, newIdentityUser3);
    }
}
