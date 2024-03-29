using Microsoft.EntityFrameworkCore;
using Application.Services;
using Application.Services.Contracts;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Application.UnitTests.FriendshipServiceTests;

public partial class FriendshipServiceTests
{
    [Fact]
    public async Task WhenAcceptFriendRequestIsCalled_ReturnsTrue()
    {
        //Arrange
        var myFriendship = new Friendship { StatusUser1Accepted = false, StatusUser2Accepted = false };

        var options = new DbContextOptions<StatusContext>();
        var chatContextMock = new Mock<StatusContext>(options);
        chatContextMock.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(1).Verifiable();

        chatContextMock.Setup(db => db.Friendships).ReturnsDbSet(new List<Friendship>());
        chatContextMock.Setup(db => db.Chats).ReturnsDbSet(new List<Chat>());

        var statusUserServiceMock = new Mock<IStatusUserService>();

        var friendshipService = new FriendshipService(
            chatContextMock.Object,
            statusUserServiceMock.Object
        );
        // Act
        var result = await friendshipService.AcceptFriendRequest(myFriendship);

        // Assert
        result.Should().BeTrue();
        chatContextMock.Verify();
    }

    [Fact]
    public async Task WhenAcceptFriendRequestIsCalled_ReturnsFalse()
    {
        //Arrange
        var myFriendship = new Friendship { StatusUser1Accepted = false, StatusUser2Accepted = false };

        var options = new DbContextOptions<StatusContext>();
        var chatContextMock = new Mock<StatusContext>(options);
        chatContextMock
            .Setup(db => db.SaveChangesAsync(default))
            .ThrowsAsync(new DbUpdateException());

        chatContextMock.Setup(db => db.Friendships).ReturnsDbSet(new List<Friendship>());
        chatContextMock.Setup(db => db.Chats).ReturnsDbSet(new List<Chat>());

        var statusUserServiceMock = new Mock<IStatusUserService>();

        var friendshipService = new FriendshipService(
            chatContextMock.Object,
            statusUserServiceMock.Object
        );
        // Act
        var result = await friendshipService.AcceptFriendRequest(myFriendship);

        // Assert
        result.Should().BeFalse();
    }
}
