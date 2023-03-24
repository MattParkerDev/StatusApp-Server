using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using StatusApp.Server.Application.Contracts;
using StatusApp.Server.Domain;
using StatusApp.Server.Infrastructure;

namespace StatusApp.Server.Presentation;

public static class FriendRoutes
{
    public static void MapFriendRoutes(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Friends").RequireAuthorization().WithOpenApi();

        group
            .MapGet(
                "/getfriends",
                async Task<Results<Ok<List<Profile>>, NoContent>> (
                    IFriendshipService friendshipService,
                    HttpContext context
                ) =>
                {
                    var userName =
                        context.User.Identity?.Name
                        ?? throw new ArgumentNullException(nameof(context.User.Identity.Name));

                    var friendsProfileList = await friendshipService.GetFriendsProfileList(
                        userName
                    );
                    return friendsProfileList.Count != 0
                        ? TypedResults.Ok(friendsProfileList)
                        : TypedResults.NoContent();
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
                    UserManager<User> userManager,
                    HttpContext context,
                    string friendUserName
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    var user = await userManager.FindByNameAsync(userName);
                    var friendUser = await userManager.FindByNameAsync(friendUserName);
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
                    UserManager<User> userManager,
                    IHubContext<StatusHub, IStatusClient> hubContext,
                    string friendUserName,
                    bool accepted
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    var user = await userManager.FindByNameAsync(userName);
                    var profile = user!.ToProfile();
                    var myFriendship = friendshipService.GetFriendship(userName, friendUserName);
                    var theirFriendship = friendshipService.GetFriendship(friendUserName, userName);

                    if (myFriendship is null || theirFriendship is null)
                        throw new ArgumentNullException();

                    if (!accepted)
                    {
                        var removeFriendshipSucceeded =
                            await friendshipService.RemoveFriendshipPair(
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

                    // Push this user and friendship to the new friend
                    await hubContext.Clients
                        .User(friendUserName)
                        .ReceiveUpdatedFriendship(theirFriendship);
                    await hubContext.Clients.User(friendUserName).ReceiveUpdatedUser(profile);
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
                        success = await friendshipService.RemoveFriendshipPair(
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
