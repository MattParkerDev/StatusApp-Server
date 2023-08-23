using Microsoft.AspNetCore.Http.HttpResults;
using Application.Contracts;
using Application.Services.Contracts;
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
                    Guid chatId
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();

                    var messages = messagingService.GetAllMessages(chatId);
                    return messages.Count != 0
                        ? TypedResults.Ok(messages)
                        : TypedResults.NoContent();
                }
            )
            .WithName("GetMessages");

        group
            .MapGet(
                "/getchatIds",
                Results<Ok<List<Guid>>, NoContent> (
                    IMessagingService messagingService,
                    HttpContext context
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();

                    var chats = messagingService.GetAllChatIdsByUserName(userName);
                    return chats.Count is 0 ? TypedResults.NoContent() : TypedResults.Ok(chats);
                }
            )
            .WithName("GetChatIds");

        group
            .MapGet(
                "/getchats",
                Results<Ok<List<Chat>>, NoContent> (
                    IMessagingService messagingService,
                    HttpContext context
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();

                    var chats = messagingService.GetAllChatsByUserName(userName);
                    return chats.Count is 0 ? TypedResults.NoContent() : TypedResults.Ok(chats);
                }
            )
            .WithName("GetChats");
    }
}
