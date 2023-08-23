using Microsoft.AspNetCore.Http.HttpResults;
using Application.Contracts;
using Domain;

namespace WebAPI.Routes;

public static class MessageRoutes
{
    public static void MapMessageRoutes(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Messages").RequireAuthorization().WithOpenApi();

        group
            .MapGet(
                "/getmessages",
                Results<Ok<List<Message>>, NoContent> (
                    IMessagingService messagingService,
                    IFriendshipService friendshipService,
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
            .WithName("GetMessages");
    }
}
