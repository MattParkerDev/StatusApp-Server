using Application.Services;
using Application.Services.Contracts;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.FriendshipServiceTests;

public partial class FriendshipServiceTests
{
    [Theory]
    [InlineData(true, true)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(false, false)]
    public void WhenGetAllFriendshipsIsCalled_ReturnsFriendshipList(
        bool user1Accepted,
        bool user2Accepted
    )
    {
        //Arrange
        var userName = "TestUserName";
        var friendship = new Friendship
        {
            UserName1 = userName,
            UserName1Accepted = user1Accepted,
            UserName2Accepted = user2Accepted
        };
        var friendships = new List<Friendship> { friendship };
        var areFriends = user1Accepted && user2Accepted;

        var options = new DbContextOptions<StatusContext>();
        var chatContextMock = new Mock<StatusContext>(options);
        chatContextMock.Setup(db => db.Friendships).ReturnsDbSet(friendships).Verifiable();

        var statusUserServiceMock = new Mock<IStatusUserService>();

        var friendshipService = new FriendshipService(
            chatContextMock.Object,
            statusUserServiceMock.Object
        );
        // Act
        var result = friendshipService.GetAllFriendships(userName, areFriends);

        // Assert
        result.Should().BeEquivalentTo(friendships);
        chatContextMock.Verify();
    }
}
