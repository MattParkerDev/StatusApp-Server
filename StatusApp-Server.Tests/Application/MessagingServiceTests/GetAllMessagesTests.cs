using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using StatusApp_Server.Application;
using StatusApp_Server.Domain;
using StatusApp_Server.Infrastructure;
using Xunit;

namespace StatusApp_Server.Tests.Application.MessagingServiceTests;

public class MessagingServiceTests
{
    [Fact]
    public void WhenGetAllMessagesIsCalled_ReturnsMessageList()
    {
        //Arrange
        var groupId = Guid.NewGuid();

        var messageList = new List<Message> { new Message { GroupId = groupId } };

        var options = new DbContextOptions<ChatContext>();
        var chatContextMock = new Mock<ChatContext>(options);
        chatContextMock.Setup(db => db.Messages).ReturnsDbSet(messageList);

        var messagingService = new MessagingService(chatContextMock.Object);

        // Act
        var result = messagingService.GetAllMessages(groupId);

        // Assert
        result.Should().BeEquivalentTo(messageList);
    }
}
