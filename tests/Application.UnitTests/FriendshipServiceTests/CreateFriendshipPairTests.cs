﻿using Application.Services;
using Application.Services.Contracts;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.FriendshipServiceTests;

public partial class FriendshipServiceTests
{
    [Fact]
    public async Task WhenCreateFriendshipPairIsCalled_ReturnsFriendship()
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
            StatusUser2Accepted = false,
        };

        var options = new DbContextOptions<StatusContext>();
        var chatContextMock = new Mock<StatusContext>(options);
        chatContextMock.Setup(db => db.Friendships).ReturnsDbSet(new List<Friendship>());

        var statusUserServiceMock = new Mock<IStatusUserService>();

        var friendshipService = new FriendshipService(
            chatContextMock.Object,
            statusUserServiceMock.Object
        );
        // Act
        var result = await friendshipService.CreateFriendship(user, friendUser);

        // Assert
        result.Should().BeEquivalentTo(expectedFriendship);
    }
}
