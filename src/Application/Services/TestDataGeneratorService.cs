using Application.Services.Contracts;
using Domain;

namespace Application.Services;

public class TestDataGeneratorService
{
    private readonly IIdentityUserService _identityUserService;
    private readonly IStatusUserService _statusUserService;
    private readonly IFriendshipService _friendshipService;
    private readonly IMessagingService _messagingService;

    public TestDataGeneratorService(
        IIdentityUserService identityUserService,
        IStatusUserService statusUserService,
        IFriendshipService friendshipService,
        IMessagingService messagingService
    )
    {
        _identityUserService = identityUserService;
        _statusUserService = statusUserService;
        _friendshipService = friendshipService;
        _messagingService = messagingService;
    }

    public async Task SeedTestData()
    {
        await GenerateTestUsersFriendshipsAndMessages();
    }

    public async Task GenerateTestUsersFriendshipsAndMessages()
    {
        var newStatusUser = new StatusUser
        {
            UserName = "BigMaurice",
            FirstName = "Maurice",
            LastName = "Smith",
            Status = "Open to Plans",
            Online = true
        };
        var newIdentityUser = new User { UserName = "BigMaurice", Email = "Bigmaurice@gmail.com" };

        var newStatusUser2 = new StatusUser
        {
            UserName = "Katie11",
            FirstName = "Katie",
            LastName = "Murray",
            Status = "Keen for dinner",
            Online = true
        };
        var newIdentityUser2 = new User { UserName = "Katie11", Email = "katie@hotmail.com" };

        var newStatusUser3 = new StatusUser
        {
            UserName = "Jrod1",
            FirstName = "Jarrod",
            LastName = "Lee",
            Status = "Quiet night in",
            Online = false
        };
        var newIdentityUser3 = new User { UserName = "Jrod1", Email = "jrod1@hotmail.com" };

        await _identityUserService.CreateUserAsync(newIdentityUser, "password1");
        await _identityUserService.CreateUserAsync(newIdentityUser2, "password1");
        await _identityUserService.CreateUserAsync(newIdentityUser3, "password1");

        await _statusUserService.CreateUserAsync(newStatusUser);
        await _statusUserService.CreateUserAsync(newStatusUser2);
        await _statusUserService.CreateUserAsync(newStatusUser3);

        // need to link the friendship to the chat

        // var chat = await _messagingService.CreateChatForUsers(
        //     new List<StatusUser> { newStatusUser, newStatusUser2 }
        // );

        var friendship1 = await _friendshipService.CreateAcceptedFriendship(
            newStatusUser,
            newStatusUser2
        );
        var friendship2 = await _friendshipService.CreateAcceptedFriendship(
            newStatusUser,
            newStatusUser3
        );

        // var message = await _messagingService.CreateMessageAsUserInGroup(
        //     newStatusUser.UserName,
        //     chat.Id,
        //     "Test Message"
        // );
    }
}
