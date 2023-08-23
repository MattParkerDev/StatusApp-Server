using Microsoft.EntityFrameworkCore;
using Application;
using Application.Contracts;
using Domain;
using Infrastructure;

namespace Application.UnitTests.FriendshipServiceTests;

public partial class FriendshipServiceTests
{
    [Fact]
    public async Task WhenAcceptFriendRequestIsCalled_ReturnsTrue()
    {
        //Arrange
        var myFriendship = new Friendship { UserName1Accepted = false, UserName2Accepted = false };

        var options = new DbContextOptions<StatusContext>();
        var chatContextMock = new Mock<StatusContext>(options);
        chatContextMock.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(1).Verifiable();

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
        var myFriendship = new Friendship { UserName1Accepted = false, UserName2Accepted = false };

        var options = new DbContextOptions<StatusContext>();
        var chatContextMock = new Mock<StatusContext>(options);
        chatContextMock
            .Setup(db => db.SaveChangesAsync(default))
            .ThrowsAsync(new DbUpdateException());

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
