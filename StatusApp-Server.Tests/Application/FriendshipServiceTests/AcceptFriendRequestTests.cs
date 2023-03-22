using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using StatusApp_Server.Application;
using StatusApp_Server.Application.Contracts;
using StatusApp_Server.Domain;
using StatusApp_Server.Infrastructure;

namespace StatusApp_Server.Tests.Application.FriendshipServiceTests;

public partial class FriendshipServiceTests
{
    [Fact]
    public async Task WhenAcceptFriendRequestIsCalled_ReturnsTrue()
    {
        //Arrange
        var myFriendship = new Friendship { Accepted = false, AreFriends = false };
        var theirFriendship = new Friendship { Accepted = true, AreFriends = false };

        var options = new DbContextOptions<StatusContext>();
        var chatContextMock = new Mock<StatusContext>(options);
        chatContextMock.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(1).Verifiable();

        var userServiceMock = new Mock<IUserService>();

        var friendshipService = new FriendshipService(
            chatContextMock.Object,
            userServiceMock.Object
        );
        // Act
        var result = await friendshipService.AcceptFriendRequest(myFriendship, theirFriendship);

        // Assert
        result.Should().BeTrue();
        chatContextMock.Verify();
    }

    [Fact]
    public async Task WhenAcceptFriendRequestIsCalled_ReturnsFalse()
    {
        //Arrange
        var myFriendship = new Friendship { Accepted = false, AreFriends = false };
        var theirFriendship = new Friendship { Accepted = true, AreFriends = false };

        var options = new DbContextOptions<StatusContext>();
        var chatContextMock = new Mock<StatusContext>(options);
        chatContextMock
            .Setup(db => db.SaveChangesAsync(default))
            .ThrowsAsync(new DbUpdateException());

        var userServiceMock = new Mock<IUserService>();

        var friendshipService = new FriendshipService(
            chatContextMock.Object,
            userServiceMock.Object
        );
        // Act
        var result = await friendshipService.AcceptFriendRequest(myFriendship, theirFriendship);

        // Assert
        result.Should().BeFalse();
    }
}
