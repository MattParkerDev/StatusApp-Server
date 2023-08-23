using Application.Contracts;
using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.FriendshipServiceTests;

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
        var friendship = new Friendship { UserName1 = userName, UserName2Accepted = areFriends ?? false };
        var friendships = new List<Friendship> { friendship };

        var options = new DbContextOptions<StatusContext>();
        var chatContextMock = new Mock<StatusContext>(options);
        chatContextMock.Setup(db => db.Friendships).ReturnsDbSet(friendships).Verifiable();

        var statusUserServiceMock = new Mock<IStatusUserService>();

        var friendshipService = new FriendshipService(
            chatContextMock.Object,
            statusUserServiceMock.Object
        );
        // Act
        var result = friendshipService.GetAllFriendships(userName, areFriends);

        // Assert
        result.Should().BeEquivalentTo(friendships);
        chatContextMock.Verify();
    }
}
