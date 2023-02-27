using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using StatusApp_Server.Application;
using StatusApp_Server.Application.Contracts;
using StatusApp_Server.Domain;
using StatusApp_Server.Infrastructure;
using Xunit;

namespace StatusApp_Server.Tests.Application.FriendshipServiceTests;

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

        var options = new DbContextOptions<ChatContext>();
        var chatContextMock = new Mock<ChatContext>(options);
        chatContextMock.Setup(db => db.Friendships).ReturnsDbSet(new List<Friendship>());

        var userServiceMock = new Mock<IUserService>();

        var friendshipService = new FriendshipService(
            chatContextMock.Object,
            userServiceMock.Object
        );
        // Act
        var result = await friendshipService.RemoveFriendshipPair(myFriendship, theirFriendship);

        // Assert
        result.Should().BeTrue();
    }
}
