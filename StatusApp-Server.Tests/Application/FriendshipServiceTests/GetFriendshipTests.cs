﻿using FluentAssertions;
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
    public void WhenGetFriendshipIsCalled_ReturnsFriendship(bool useGroupId)
    {
        //Arrange
        var userName = "TestUserName";
        var friendUserName = "AnotherUserName";
        var groupId = Guid.NewGuid();
        var friendship = new Friendship
        {
            UserName = userName,
            FriendUserName = friendUserName,
            GroupId = groupId
        };
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
        var result = !useGroupId
            ? friendshipService.GetFriendship(userName, friendUserName)
            : friendshipService.GetFriendship(userName, groupId);

        // Assert
        result.Should().Be(friendship);
        chatContextMock.Verify();
    }
}
