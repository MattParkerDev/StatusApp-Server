using Microsoft.EntityFrameworkCore;
using StatusApp.Server.Application;
using StatusApp.Server.Domain;
using StatusApp.Server.Infrastructure;

namespace StatusApp.Server.Tests.Application.MessagingServiceTests;

public class MessagingServiceTests
{
    [Fact]
    public void WhenGetAllMessagesIsCalled_ReturnsMessageList()
    {
        //Arrange
        var groupId = Guid.NewGuid();

        var messageList = new List<Message> { new Message { GroupId = groupId } };

        var options = new DbContextOptions<StatusContext>();
        var chatContextMock = new Mock<StatusContext>(options);
        chatContextMock.Setup(db => db.Messages).ReturnsDbSet(messageList);

        var messagingService = new MessagingService(chatContextMock.Object);

        // Act
        var result = messagingService.GetAllMessages(groupId);

        // Assert
        result.Should().BeEquivalentTo(messageList);
    }
}
