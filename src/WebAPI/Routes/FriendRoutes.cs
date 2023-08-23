using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Application.Contracts;
using Domain;
using Domain.DTOs;
using WebAPI.SignalR;

namespace WebAPI.Routes;

public static class FriendRoutes
{
    public static void MapFriendRoutes(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Friends").RequireAuthorization().WithOpenApi();

        group
            .MapGet(
                "/getfriends",
                async Task<Ok<List<StatusUserDto>>> (
                    IFriendshipService friendshipService,
                    HttpContext context
                ) =>
                {
                    var userName =
                        context.User.Identity?.Name
                        ?? throw new ArgumentNullException(nameof(context.User.Identity.Name));

                    var friendsDtoList = await friendshipService.GetFriendsDtoList(userName);
                    return TypedResults.Ok(friendsDtoList);
                }
            )
            .WithName("GetFriends");

        group
            .MapGet(
                "/getfriendships",
                Results<Ok<List<Friendship>>, NoContent> (
                    HttpContext context,
                    IFriendshipService friendshipService,
                    bool? areFriends
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    var friendships = friendshipService.GetAllFriendships(userName, areFriends);
                    return friendships.Count != 0
                        ? TypedResults.Ok(friendships)
                        : TypedResults.NoContent();
                }
            )
            .WithName("GetFriendships");

        group
            .MapPut(
                "/sendfriendrequest",
                async Task<Results<Ok<Friendship>, NotFound, Conflict>> (
                    IFriendshipService friendshipService,
                    IStatusUserService statusUserService,
                    HttpContext context,
                    string friendUserName
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    var user = await statusUserService.GetUserByNameAsync(userName);
                    var friendUser = await statusUserService.GetUserByNameAsync(friendUserName);
                    if (friendUser == null || user == null)
                    {
                        return TypedResults.NotFound();
                    }

                    var myFriendship = await friendshipService.CreateFriendshipPair(
                        user,
                        friendUser
                    );

                    if (myFriendship is null)
                    {
                        return TypedResults.Conflict();
                    }

                    return TypedResults.Ok(myFriendship);
                }
            )
            .WithName("SendFriendRequest");

        group
            .MapPut(
                "/actionfriendrequest",
                async Task<Results<Ok<Friendship>, Ok, Conflict>> (
                    HttpContext context,
                    IFriendshipService friendshipService,
                    IStatusUserService statusUserService,
                    IHubContext<StatusHub, IStatusClient> hubContext,
                    string friendUserName,
                    bool accepted
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    var statusUser = await statusUserService.GetUserByNameAsync(userName);

                    var myFriendship = friendshipService.GetFriendship(userName, friendUserName);
                    var theirFriendship = friendshipService.GetFriendship(friendUserName, userName);

                    if (myFriendship is null || theirFriendship is null)
                        throw new ArgumentNullException();

                    if (!accepted)
                    {
                        var removeFriendshipSucceeded =
                            await friendshipService.DeleteFriendshipPair(
                                myFriendship,
                                theirFriendship
                            );
                        //TODO: Consider SignalR Push
                        return removeFriendshipSucceeded
                            ? TypedResults.Ok()
                            : TypedResults.Conflict();
                    }

                    var acceptSucceeded = await friendshipService.AcceptFriendRequest(
                        myFriendship,
                        theirFriendship
                    );
                    if (acceptSucceeded is false)
                        return TypedResults.Conflict();

                    var statusUserDto = statusUser!.ToDto();

                    // Push this user and friendship to the new friend
                    await hubContext.Clients
                        .User(friendUserName)
                        .ReceiveUpdatedFriendship(theirFriendship);
                    await hubContext.Clients.User(friendUserName).ReceiveUpdatedUser(statusUserDto);
                    return TypedResults.Ok(myFriendship);
                }
            )
            .WithName("ActionFriendRequest");

        group
            .MapDelete(
                "/removefriend",
                async Task<Results<Ok, Conflict>> (
                    IFriendshipService friendshipService,
                    HttpContext context,
                    IHubContext<StatusHub, IStatusClient> hubContext,
                    string friendUserName
                ) =>
                {
                    var success = false;
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    var myFriendship = friendshipService.GetFriendship(userName, friendUserName);
                    var theirFriendship = friendshipService.GetFriendship(friendUserName, userName);

                    if (myFriendship != null && theirFriendship != null)
                    {
                        success = await friendshipService.DeleteFriendshipPair(
                            myFriendship,
                            theirFriendship
                        );
                    }

                    if (success is not true)
                    {
                        return TypedResults.Conflict();
                    }

                    // Delete this user from friend's list
                    await hubContext.Clients.User(friendUserName).DeleteFriend(userName);
                    return TypedResults.Ok();
                }
            )
            .WithName("RemoveFriend");
    }
}
