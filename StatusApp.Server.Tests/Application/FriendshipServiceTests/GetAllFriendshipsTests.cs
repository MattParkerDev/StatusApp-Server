using Microsoft.EntityFrameworkCore;
using StatusApp.Server.Application;
using StatusApp.Server.Application.Contracts;
using StatusApp.Server.Domain;
using StatusApp.Server.Infrastructure;

namespace StatusApp.Server.Tests.Application.FriendshipServiceTests;

public partial class FriendshipServiceTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void WhenGetAllFriendshipsIsCalled_ReturnsFriendshipList(bool? areFriends)
    {
        //Arrange
        var userName = "TestUserName";
        var friendship = new Friendship { UserName = userName, AreFriends = areFriends ?? false };
        var friendships = new List<Friendship> { friendship };

        var options = new DbContextOptions<StatusContext>();
        var chatContextMock = new Mock<StatusContext>(options);
        chatContextMock.Setup(db => db.Friendships).ReturnsDbSet(friendships).Verifiable();

        var userServiceMock = new Mock<IUserService>();

        var friendshipService = new FriendshipService(
            chatContextMock.Object,
            userServiceMock.Object
        );
        // Act
        var result = friendshipService.GetAllFriendships(userName, areFriends);

        // Assert
        result.Should().BeEquivalentTo(friendships);
        chatContextMock.Verify();
    }
}
