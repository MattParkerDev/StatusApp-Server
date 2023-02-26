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
    public async Task WhenCreateFriendshipPairIsCalled_ReturnsUsernameList()
    {
        //Arrange
        var userName = "TestUserName";
        var friendUserName = "AnotherUserName";

        List<Friendship> friendshipList = new List<Friendship>();

        var user = new User
        {
            UserName = userName,
            FirstName = "AName",
            LastName = "LastName",
            Status = "My Status",
            Online = false
        };

        var friendUser = new User
        {
            UserName = friendUserName,
            FirstName = "FriendsName",
            LastName = "FriendsLastName",
            Status = "Another Status",
            Online = true
        };

        var expectedFriendship = new Friendship
        {
            UserName = user.UserName!,
            FriendUserName = friendUser.UserName!,
            Accepted = true,
            AreFriends = false,
            FriendFirstName = friendUser.FirstName,
            FriendLastName = friendUser.LastName,
        };

        var options = new DbContextOptions<ChatContext>();
        var chatContextMock = new Mock<ChatContext>(options);
        chatContextMock.Setup(db => db.Friendships).ReturnsDbSet(friendshipList);

        var userServiceMock = new Mock<IUserService>();

        var friendshipService = new FriendshipService(
            chatContextMock.Object,
            userServiceMock.Object
        );
        // Act
        var result = await friendshipService.CreateFriendshipPair(user, friendUser);

        // Assert
        result.Should().BeEquivalentTo(expectedFriendship);
    }
}
