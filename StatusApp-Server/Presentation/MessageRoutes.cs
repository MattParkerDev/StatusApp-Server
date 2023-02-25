using Microsoft.AspNetCore.Http.HttpResults;
using StatusApp_Server.Application;
using StatusApp_Server.Application.Contracts;
using StatusApp_Server.Domain;

namespace StatusApp_Server.Presentation;

public static class MessageRoutes
{
    public static void MapMessageRoutes(this WebApplication app)
    {
        app.MapGet(
                "/getmessages",
                Results<Ok<List<Message>>, NoContent> (
                    IMessagingService messagingService,
                    FriendshipService friendshipService,
                    HttpContext context,
                    Guid groupId
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();

                    var friendship = friendshipService.GetFriendship(userName, groupId);
                    if (friendship is null)
                        return TypedResults.NoContent();

                    var messages = messagingService.GetAllMessages(groupId);
                    return messages.Count != 0
                        ? TypedResults.Ok(messages)
                        : TypedResults.NoContent();
                }
            )
            .RequireAuthorization()
            .WithName("GetMessages")
            .WithOpenApi();
    }
}
