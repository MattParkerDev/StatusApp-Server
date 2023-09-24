using Application.DTOs;
using Application.Mappers;
using Microsoft.AspNetCore.Http.HttpResults;
using Application.Services.Contracts;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Routes;

public static class MessageRoutes
{
    public static void MapMessageRoutes(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Messages").RequireAuthorization().WithOpenApi();

        group
            .MapGet(
                "/getmessages",
                Results<Ok<List<MessageDto>>, NoContent> (
                    IMessagingService messagingService,
                    HttpContext context,
                    Guid chatId
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();

                    var messages = messagingService.GetAllMessages(new ChatId(chatId));
                    var messageDtos = messages.Select(s => s.ToDto()).ToList();
                    return messages.Count != 0
                        ? TypedResults.Ok(messageDtos)
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
                    return chats.Count is 0
                        ? TypedResults.NoContent()
                        : TypedResults.Ok(chats.Select(s => s.Value).ToList());
                }
            )
            .WithName("GetChatIds");

        group
            .MapGet(
                "/getchats",
                Results<Ok<List<ChatDto>>, NoContent> (
                    IMessagingService messagingService,
                    HttpContext context
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();

                    var chats = messagingService.GetAllChatsByUserName(userName);
                    var chatDtos = chats.Select(s => s.ToDto()).ToList();
                    return chatDtos.Count is 0
                        ? TypedResults.NoContent()
                        : TypedResults.Ok(chatDtos);
                }
            )
            .WithName("GetChats");
    }
}
