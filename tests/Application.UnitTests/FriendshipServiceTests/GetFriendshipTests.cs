using Application.Services;
using Application.Services.Contracts;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.FriendshipServiceTests;

public partial class FriendshipServiceTests
{
    [Fact]
    public void WhenGetFriendshipIsCalled_ReturnsFriendship()
    {
        //Arrange
        var userName = "TestUserName";
        var friendUserName = "AnotherUserName";
        var groupId = Guid.NewGuid();
        var friendship = new Friendship { StatusUser1Id = userName, StatusUser2Id = friendUserName, };
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
        var result = friendshipService.GetFriendship(userName, friendUserName);

        // Assert
        result.Should().Be(friendship);
        chatContextMock.Verify();
    }
}
