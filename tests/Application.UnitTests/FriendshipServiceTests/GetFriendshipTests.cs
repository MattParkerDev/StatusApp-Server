using Application.Contracts;
using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.FriendshipServiceTests;

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

        var options = new DbContextOptions<StatusContext>();
        var chatContextMock = new Mock<StatusContext>(options);
        chatContextMock.Setup(db => db.Friendships).ReturnsDbSet(friendships).Verifiable();

        var statusUserServiceMock = new Mock<IStatusUserService>();

        var friendshipService = new FriendshipService(
            chatContextMock.Object,
            statusUserServiceMock.Object
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
