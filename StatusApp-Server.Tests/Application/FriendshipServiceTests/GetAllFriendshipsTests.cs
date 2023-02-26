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

        var options = new DbContextOptions<ChatContext>();
        var chatContextMock = new Mock<ChatContext>(options);
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
