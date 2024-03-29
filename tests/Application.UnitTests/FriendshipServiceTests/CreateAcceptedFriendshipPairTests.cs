﻿using Application.Services;
using Application.Services.Contracts;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.FriendshipServiceTests;

public partial class FriendshipServiceTests
{
    [Fact]
    public async Task WhenCreateAcceptedFriendshipPairIsCalled_ReturnsFriendship()
    {
        //Arrange
        var userName = "TestUserName";
        var friendUserName = "AnotherUserName";

        var user = new StatusUser
        {
            UserName = userName,
            FirstName = "AName",
            LastName = "LastName",
            Status = "My Status",
            Online = false
        };

        var friendUser = new StatusUser
        {
            UserName = friendUserName,
            FirstName = "FriendsName",
            LastName = "FriendsLastName",
            Status = "Another Status",
            Online = true
        };

        var expectedFriendship = new Friendship
        {
            StatusUser1Id = user.UserName,
            StatusUser2Id = friendUser.UserName,
            StatusUser1Accepted = true,
            StatusUser2Accepted = true,
            BecameFriendsDate = It.IsAny<DateTime>(),
        };

        var options = new DbContextOptions<StatusContext>();
        var chatContextMock = new Mock<StatusContext>(options);
        chatContextMock.Setup(db => db.Friendships).ReturnsDbSet(new List<Friendship>());
        chatContextMock.Setup(db => db.Chats).ReturnsDbSet(new List<Chat>());

        var statusUserServiceMock = new Mock<IStatusUserService>();

        var friendshipService = new FriendshipService(
            chatContextMock.Object,
            statusUserServiceMock.Object
        );
        // Act
        var result = await friendshipService.CreateAcceptedFriendship(user, friendUser);

        // Assert
        result.Should().BeOfType<Friendship>();
        result!.StatusUser1Id.Should().Be(expectedFriendship.StatusUser1Id);
        result.StatusUser2Id.Should().Be(expectedFriendship.StatusUser2Id);
        result.StatusUser1Accepted.Should().Be(expectedFriendship.StatusUser1Accepted);
        result.StatusUser2Accepted.Should().Be(expectedFriendship.StatusUser2Accepted);
    }
}
