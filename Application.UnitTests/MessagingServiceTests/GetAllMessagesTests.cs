using Microsoft.EntityFrameworkCore;
using Application;
using Domain;
using Infrastructure;

namespace Application.UnitTests.MessagingServiceTests;

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
