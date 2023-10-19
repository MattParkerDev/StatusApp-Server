using Microsoft.EntityFrameworkCore;
using Application.Services;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Application.UnitTests.MessagingServiceTests;

public class MessagingServiceTests
{
    [Fact]
    public void WhenGetAllMessagesIsCalled_ReturnsMessageList()
    {
        //Arrange
        var chatId = new ChatId(Guid.NewGuid());
        var messageList = new List<Message> { new Message { ChatId = chatId } };

        var options = new DbContextOptions<StatusContext>();
        var chatContextMock = new Mock<StatusContext>(options);
        chatContextMock.Setup(db => db.Messages).ReturnsDbSet(messageList);

        var messagingService = new MessagingService(chatContextMock.Object);

        // Act
        var result = messagingService.GetAllMessages(chatId);

        // Assert
        result.Should().BeEquivalentTo(messageList);
    }
}
