using Application.Contracts;
using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.FriendshipServiceTests;

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

        var statusUserServiceMock = new Mock<IStatusUserService>();

        var friendshipService = new FriendshipService(
            chatContextMock.Object,
            statusUserServiceMock.Object
        );
        // Act
        var result = await friendshipService.DeleteFriendshipPair(myFriendship, theirFriendship);

        // Assert
        result.Should().BeTrue();
    }
}
