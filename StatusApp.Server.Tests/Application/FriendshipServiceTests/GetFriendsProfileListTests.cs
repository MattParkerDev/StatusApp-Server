using Microsoft.EntityFrameworkCore;
using StatusApp.Server.Application;
using StatusApp.Server.Application.Contracts;
using StatusApp.Server.Domain;
using StatusApp.Server.Infrastructure;

namespace StatusApp.Server.Tests.Application.FriendshipServiceTests;

public partial class FriendshipServiceTests
{
    [Fact]
    public async Task WhenGetFriendsProfileListIsCalled_ReturnsProfileListAsync()
    {
        //Arrange
        var userName = "TestUserName";
        var friendUserName = "AnotherUserName";
        var friendship = new Friendship
        {
            UserName = userName,
            FriendUserName = friendUserName,
            AreFriends = true
        };
        var friendships = new List<Friendship> { friendship };

        var friendProfile = new Profile
        {
            UserName = friendUserName,
            FirstName = "AName",
            LastName = "LastName",
            Status = "My Status",
            Online = false
        };

        var friendProfileList = new List<Profile> { friendProfile };

        var options = new DbContextOptions<StatusContext>();
        var chatContextMock = new Mock<StatusContext>(options);
        chatContextMock.Setup(db => db.Friendships).ReturnsDbSet(friendships).Verifiable();

        var userServiceMock = new Mock<IUserService>();
        userServiceMock
            .Setup(x => x.GetUserByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(
                new User
                {
                    UserName = friendUserName,
                    FirstName = "AName",
                    LastName = "LastName",
                    Status = "My Status",
                    Online = false
                }
            );

        //TODO: Implement something similar to this so GetFriendsUserNameList can be mocked in GetFriendProfileList
        //      Basically Mocking some methods in the tested class
        // var friendshipServiceMock = new Mock<IFriendshipService>();
        // friendshipServiceMock
        //     .Setup(s => s.GetFriendsUserNameList(userName))
        //     .Returns(new List<string> { friendUserName });

        var friendshipService = new FriendshipService(
            chatContextMock.Object,
            userServiceMock.Object
        );
        // Act
        var result = await friendshipService.GetFriendsProfileList(userName);

        // Assert
        result.Should().BeEquivalentTo(friendProfileList);
        chatContextMock.Verify();
    }
}
