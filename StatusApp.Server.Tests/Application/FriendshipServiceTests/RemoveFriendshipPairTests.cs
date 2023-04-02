using Microsoft.EntityFrameworkCore;
using StatusApp.Server.Application;
using StatusApp.Server.Application.Contracts;
using StatusApp.Server.Domain;
using StatusApp.Server.Infrastructure;

namespace StatusApp.Server.Tests.Application.FriendshipServiceTests;

public partial class FriendshipServiceTests
{
    [Fact]
    public async Task WhenRemoveFriendshipPairIsCalled_ReturnsTrueAsync()
    {
        //Arrange
        var userName = "TestUserName";
        var friendUserName = "AnotherUserName";

        var myFriendship = new Friendship { UserName = userName, FriendUserName = friendUserName };

        var theirFriendship = new Friendship
        {
            UserName = userName,
            FriendUserName = friendUserName
        };

        var options = new DbContextOptions<StatusContext>();
        var chatContextMock = new Mock<StatusContext>(options);
        chatContextMock.Setup(db => db.Friendships).ReturnsDbSet(new List<Friendship>());

        var identityUserServiceMock = new Mock<IIdentityUserService>();
        var statusUserServiceMock = new Mock<IStatusUserService>();

        var friendshipService = new FriendshipService(
            chatContextMock.Object,
            identityUserServiceMock.Object,
            statusUserServiceMock.Object
        );
        // Act
        var result = await friendshipService.DeleteFriendshipPair(myFriendship, theirFriendship);

        // Assert
        result.Should().BeTrue();
    }
}
