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
    public async Task WhenCreateAcceptedFriendshipPairIsCalled_ReturnsFriendship()
    {
        //Arrange
        var userName = "TestUserName";
        var friendUserName = "AnotherUserName";

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
            AreFriends = true,
            BecameFriendsDate = It.IsAny<DateTime>(),
            FriendFirstName = friendUser.FirstName,
            FriendLastName = friendUser.LastName,
            GroupId = It.IsAny<Guid>()
        };

        var options = new DbContextOptions<StatusContext>();
        var chatContextMock = new Mock<StatusContext>(options);
        chatContextMock.Setup(db => db.Friendships).ReturnsDbSet(new List<Friendship>());

        var userServiceMock = new Mock<IUserService>();

        var friendshipService = new FriendshipService(
            chatContextMock.Object,
            userServiceMock.Object
        );
        // Act
        var result = await friendshipService.CreateAcceptedFriendshipPair(user, friendUser);

        // Assert
        result.Should().BeOfType<Friendship>();
        result!.UserName.Should().Be(expectedFriendship.UserName);
        result.FriendUserName.Should().Be(expectedFriendship.FriendUserName);
        result.Accepted.Should().Be(expectedFriendship.Accepted);
        result.AreFriends.Should().Be(expectedFriendship.AreFriends);
        result.FriendFirstName.Should().Be(expectedFriendship.FriendFirstName);
        result.FriendLastName.Should().Be(expectedFriendship.FriendLastName);
    }
}
