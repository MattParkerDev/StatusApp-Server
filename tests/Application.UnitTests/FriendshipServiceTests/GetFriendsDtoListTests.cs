using Application.DTOs;
using Application.Services;
using Application.Services.Contracts;
using Domain;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTests.FriendshipServiceTests;

public partial class FriendshipServiceTests
{
    [Fact]
    public async Task WhenGetFriendsDtoListIsCalled_ReturnsDtoListAsync()
    {
        //Arrange
        var userName = "TestUserName";
        var friendUserName = "AnotherUserName";
        var friendship = new Friendship
        {
            UserName1 = userName,
            UserName2 = friendUserName,
            UserName2Accepted = true
        };
        var friendships = new List<Friendship> { friendship };

        var statusUserDto = new StatusUserDto
        {
            UserName = friendUserName,
            FirstName = "AName",
            LastName = "LastName",
            Status = "My Status",
            Online = false
        };

        var friendsDtoList = new List<StatusUserDto> { statusUserDto };

        var options = new DbContextOptions<StatusContext>();
        var chatContextMock = new Mock<StatusContext>(options);
        chatContextMock.Setup(db => db.Friendships).ReturnsDbSet(friendships).Verifiable();

        var statusUserServiceMock = new Mock<IStatusUserService>();

        statusUserServiceMock
            .Setup(x => x.GetUserByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(
                new StatusUser
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
            statusUserServiceMock.Object
        );
        // Act
        var result = await friendshipService.GetFriendsDtoList(userName);

        // Assert
        result.Should().BeEquivalentTo(friendsDtoList);
        chatContextMock.Verify();
    }
}
