using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using StatusApp_Server.Application;
using StatusApp_Server.Application.Contracts;
using StatusApp_Server.Domain;
using StatusApp_Server.Infrastructure;

namespace StatusApp_Server.Tests.Application;

public class FriendshipServiceTests
{
    [Fact]
    public async Task WhenAcceptFriendRequestIsCalled_ReturnsTrue()
    {
        //Arrange
        var myFriendship = new Friendship { Accepted = false, AreFriends = false };
        var theirFriendship = new Friendship { Accepted = true, AreFriends = false };

        var options = new DbContextOptions<ChatContext>();
        var chatContextMock = new Mock<ChatContext>(options);
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
}
