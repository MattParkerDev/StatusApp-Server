using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using StatusApp_Server.Application;
using StatusApp_Server.Domain;
using StatusApp_Server.Infrastructure;

namespace StatusApp_Server.Presentation;

public static class FriendRoutes
{
    public static void MapFriendRoutes(this WebApplication app)
    {
        app.MapGet(
                "/getfriends",
                async (FriendshipService friendshipService, HttpContext context) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    var friendsProfileList = await friendshipService.GetFriendsProfileList(
                        userName
                    );
                    return friendsProfileList.Count() != 0
                        ? Results.Ok(friendsProfileList)
                        : Results.NoContent();
                }
            )
            .RequireAuthorization()
            .WithName("GetFriends")
            .WithOpenApi();

        app.MapGet(
                "/getfriendships",
                (ChatContext db, HttpContext context, bool? areFriends) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    var friendships =
                        areFriends == null // Optional AreFriends returns all friendships regardless of status if not supplied in request
                            ? db.Friendships.Where(s => s.UserName == userName)
                            : db.Friendships.Where(
                                s => s.UserName == userName && s.AreFriends == areFriends
                            );
                    return friendships.Count() != 0 ? Results.Ok(friendships) : Results.NoContent();
                }
            )
            .RequireAuthorization()
            .WithName("GetFriendships")
            .WithOpenApi();

        app.MapPut(
                "/sendfriendrequest",
                async (
                    ChatContext db,
                    UserManager<User> userManager,
                    HttpContext context,
                    string friendUserName
                ) =>
                {
                    var success = false;
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    User? user = await userManager.FindByNameAsync(userName);
                    User? friendUser = await userManager.FindByNameAsync(friendUserName);
                    if (friendUser == null || user == null)
                    {
                        return Results.NotFound();
                    }

                    var myFriendship = new Friendship
                    {
                        UserName = userName,
                        FriendUserName = friendUser.UserName,
                        Accepted = true,
                        AreFriends = false,
                        FriendFirstName = friendUser.FirstName,
                        FriendLastName = friendUser.LastName,
                    };
                    var theirFriendship = new Friendship
                    {
                        UserName = friendUser.UserName,
                        FriendUserName = userName,
                        Accepted = false,
                        AreFriends = false,
                        FriendFirstName = user.FirstName,
                        FriendLastName = user.LastName,
                    };
                    db.Friendships.Add(myFriendship);
                    db.Friendships.Add(theirFriendship);
                    try
                    {
                        await db.SaveChangesAsync();
                        success = true;
                    }
                    catch (Exception e)
                    {
                        var errorString = $"Error: {e.Message}";
                    }

                    return success ? Results.Ok(myFriendship) : Results.Conflict(myFriendship);
                }
            )
            .RequireAuthorization()
            .WithName("SendFriendRequest")
            .WithOpenApi();

        app.MapPut(
                "/actionfriendrequest",
                async (
                    HttpContext context,
                    FriendshipService friendshipService,
                    UserManager<User> userManager,
                    IHubContext<StatusHub, IStatusClient> hubContext,
                    string friendUserName,
                    bool accepted
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    var user = await userManager.FindByNameAsync(userName);
                    var profile = user.ToProfile();
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
                        return removeFriendshipSucceeded ? Results.Ok() : Results.Conflict();
                    }

                    var acceptSucceeded = await friendshipService.AcceptFriendRequest(
                        myFriendship,
                        theirFriendship
                    );
                    if (acceptSucceeded is false)
                        return Results.Conflict();

                    // Push this user and friendship to the new friend
                    await hubContext.Clients
                        .User(friendUserName)
                        .ReceiveUpdatedFriendship(theirFriendship);
                    await hubContext.Clients.User(friendUserName).ReceiveUpdatedUser(profile);
                    return Results.Ok(myFriendship);
                }
            )
            .RequireAuthorization()
            .WithName("ActionFriendRequest")
            .WithOpenApi();

        app.MapDelete(
                "/removefriend",
                async (
                    ChatContext db,
                    HttpContext context,
                    IHubContext<StatusHub, IStatusClient> hubContext,
                    string friendUserName
                ) =>
                {
                    var success = false;
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    var myFriendship = db.Friendships.FirstOrDefault(
                        s => s.UserName == userName && s.FriendUserName == friendUserName
                    );
                    var theirFriendship = db.Friendships.FirstOrDefault(
                        s => s.UserName == friendUserName && s.FriendUserName == userName
                    );
                    if (myFriendship != null && theirFriendship != null)
                    {
                        db.Friendships.Remove(myFriendship);
                        db.Friendships.Remove(theirFriendship);
                    }

                    try
                    {
                        await db.SaveChangesAsync();
                        success = true;
                    }
                    catch (Exception e)
                    {
                        var errorString = $"Error: {e.Message}";
                    }

                    if (success != true)
                    {
                        return Results.Conflict();
                    }

                    // Delete this user from friend's list
                    await hubContext.Clients.User(friendUserName).DeleteFriend(userName);
                    return Results.Ok();
                }
            )
            .RequireAuthorization()
            .WithName("RemoveFriend")
            .WithOpenApi();
    }
}
