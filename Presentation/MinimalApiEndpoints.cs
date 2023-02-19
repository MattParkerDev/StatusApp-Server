using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using StatusApp_Server.Application;
using StatusApp_Server.Application.Contracts;
using StatusApp_Server.Domain;
using StatusApp_Server.Infrastructure;

namespace StatusApp_Server.Presentation;

public static class MinimalApiEndpoints
{
    public static void RegisterAuthAPIs(this WebApplication app)
    {
        app.MapGet(
                "/checkAuth",
                (HttpContext context) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    return userName;
                }
            )
            .RequireAuthorization()
            .WithName("CheckAuth")
            .WithOpenApi();
    }

    public static void RegisterMessageAPIs(this WebApplication app)
    {
        app.MapGet(
                "/getmessages",
                (
                    IMessagingService messagingService,
                    FriendshipService friendshipService,
                    HttpContext context,
                    Guid groupId
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();

                    var friendship = friendshipService.GetFriendship(groupId, userName);
                    if (friendship is null)
                        return Results.NoContent();

                    var messages = messagingService.GetAllMessages(groupId);
                    return messages.Count != 0 ? Results.Ok(messages) : Results.NoContent();
                }
            )
            .RequireAuthorization()
            .WithName("GetMessages")
            .WithOpenApi();

        // app.MapPut(
        //         "/pushMessage",
        //         async (
        //             ChatContext db,
        //             DateTime Created,
        //             string Data,
        //             Guid GroupId,
        //             string Author
        //         ) =>
        //         {
        //             var incomingMessage = new Message();
        //             var success = false;
        //             incomingMessage.Data = Data;
        //             incomingMessage.GroupId = GroupId;
        //             incomingMessage.Created = Created;
        //             incomingMessage.AuthorUserName = Author;
        //             db.Messages.Add(incomingMessage);
        //             try
        //             {
        //                 await db.SaveChangesAsync();
        //                 success = true;
        //             }
        //             catch (Exception e)
        //             {
        //                 var errorString = $"Error: {e.Message}";
        //                 throw;
        //             }
        //
        //             return success == true
        //                 ? Results.Ok(incomingMessage)
        //                 : Results.Conflict(incomingMessage);
        //         }
        //     )
        //     .RequireAuthorization()
        //     .WithName("PushMessage")
        //     .WithOpenApi();
        // app.MapDelete(
        //         "deleteMessage",
        //         async (ChatContext db, int MessageId) =>
        //         {
        //             var targetMessage = db.Messages.First(s => s.MessageId == MessageId);
        //             db.Messages.Remove(targetMessage);
        //             await db.SaveChangesAsync();
        //             return Results.Ok(targetMessage);
        //         }
        //     )
        //     .RequireAuthorization()
        //     .WithName("DeleteMessage")
        //     .WithOpenApi();
        //
        // app.MapPatch(
        //         "updateMessage",
        //         async (ChatContext db, int MessageId, string Data, DateTime LastUpdated) =>
        //         {
        //             var targetMessage = db.Messages.First(s => s.MessageId == MessageId);
        //             targetMessage.LastUpdated = LastUpdated;
        //             targetMessage.Data = Data;
        //             await db.SaveChangesAsync();
        //             return Results.Ok(targetMessage);
        //         }
        //     )
        //     .RequireAuthorization()
        //     .WithName("UpdateMessage")
        //     .WithOpenApi();
    }

    public static void RegisterUserAPIs(this WebApplication app)
    {
        app.MapGet(
                "/getUser",
                async (ChatContext db, UserManager<User> userManager, HttpContext context) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    User? user = await userManager.FindByNameAsync(userName);
                    if (user is null)
                    {
                        return Results.NotFound();
                    }

                    var profile = new Profile
                    {
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Status = user.Status,
                        Online = user.Online
                    };
                    return Results.Ok(profile);
                    //return Results.Ok(user);
                }
            )
            .RequireAuthorization()
            .WithName("GetUser")
            .WithOpenApi();

        app.MapGet(
                "/signin",
                async (
                    ChatContext db,
                    UserManager<User> userManager,
                    SignInManager<User> signInManager,
                    string userName,
                    string password
                ) =>
                {
                    var user = await userManager.FindByNameAsync(userName);
                    if (user == null)
                    {
                        return Results.BadRequest();
                    }

                    var profile = new Profile
                    {
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Status = user.Status,
                        Online = user.Online
                    };

                    var success = await signInManager.PasswordSignInAsync(
                        user,
                        password,
                        true,
                        false
                    );
                    return success.Succeeded ? Results.Ok(profile) : Results.Unauthorized();
                }
            )
            .AllowAnonymous()
            .WithName("SignIn")
            .WithOpenApi();

        app.MapGet(
                "/signOut",
                async (SignInManager<User> signInManager) =>
                {
                    await signInManager.SignOutAsync();
                    return Results.Ok();
                }
            )
            .RequireAuthorization()
            .WithName("SignOut")
            .WithOpenApi();

        app.MapPut(
                "/createUser",
                async (
                    UserManager<User> userManager,
                    SignInManager<User> signInManager,
                    string userName,
                    string password,
                    string firstName,
                    string lastName,
                    string email
                ) =>
                {
                    var newUser = new User
                    {
                        UserName = userName,
                        Email = email,
                        FirstName = firstName,
                        LastName = lastName
                    };

                    var result = await userManager.CreateAsync(newUser, password);

                    if (!result.Succeeded)
                    {
                        return Results.BadRequest(result.Errors);
                    }

                    var newProfile = new Profile
                    {
                        UserName = userName,
                        FirstName = firstName,
                        LastName = lastName
                    };
                    await signInManager.SignInAsync(newUser, isPersistent: true);
                    return Results.Ok(newProfile);
                }
            )
            .AllowAnonymous()
            .WithName("CreateUser")
            .WithOpenApi();

        app.MapDelete(
                "deleteUser",
                async (
                    HttpContext context,
                    UserManager<User> userManager,
                    SignInManager<User> signInManager
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    var targetUser = await userManager.FindByNameAsync(userName);
                    if (targetUser is null)
                    {
                        return Results.BadRequest();
                    }

                    //TODO: Confirm Auth flow
                    await signInManager.SignOutAsync();
                    await userManager.DeleteAsync(targetUser);
                    //TODO: Also delete Friendships
                    return Results.Ok();
                }
            )
            .RequireAuthorization()
            .WithName("DeleteUser")
            .WithOpenApi();

        //TODO: Create separate route for updating Password
        app.MapPatch(
                "updateUser",
                async (
                    ChatContext db,
                    IHubContext<StatusHub, IStatusClient> hubContext,
                    HttpContext context,
                    UserManager<User> userManager,
                    FriendshipService friendshipService,
                    string? firstName,
                    string? lastName,
                    string? status,
                    bool? online
                ) =>
                {
                    var userName = context.User.Identity?.Name ?? throw new ArgumentNullException();
                    //TODO: Update Friendships too
                    var targetUser = await userManager.FindByNameAsync(userName);
                    if (targetUser is null)
                    {
                        return Results.BadRequest();
                    }

                    targetUser.FirstName = firstName ?? targetUser.FirstName;
                    targetUser.LastName = lastName ?? targetUser.LastName;
                    targetUser.Status = status ?? targetUser.Status;
                    targetUser.Online = online ?? targetUser.Online;
                    await userManager.UpdateAsync(targetUser);

                    var updatedProfile = new Profile
                    {
                        UserName = targetUser.UserName,
                        FirstName = targetUser.FirstName,
                        LastName = targetUser.LastName,
                        Status = targetUser.Status,
                        Online = targetUser.Online,
                    };

                    // Push changes to this user to any of their friends
                    var friendsUserNameList = friendshipService.GetFriendsUserNameList(userName);
                    var usersToNotify = db.Connections
                        .Where(s => friendsUserNameList.Contains(s.UserName))
                        .GroupBy(s => s.UserName)
                        .Select(s => s.First().UserName)
                        .ToList();

                    await hubContext.Clients
                        .Users(usersToNotify)
                        .ReceiveUpdatedUser(updatedProfile);
                    return Results.Ok(updatedProfile);
                }
            )
            .RequireAuthorization()
            .WithName("UpdateUser")
            .WithOpenApi();
    }

    public static void RegisterFriendAPIs(this WebApplication app)
    {
        app.MapGet(
                "/getfriends",
                async (
                    ChatContext db,
                    FriendshipService friendshipService,
                    UserManager<User> userManager,
                    HttpContext context
                ) =>
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
                    if (myFriendship != null && theirFriendship != null)
                    {
                        if (accepted)
                        {
                            var success = await friendshipService.AcceptFriendRequest(
                                myFriendship,
                                theirFriendship
                            );
                            if (success is false)
                                return Results.Conflict();
                        }
                        else
                        {
                            var success = await friendshipService.RemoveFriendshipPair(
                                myFriendship,
                                theirFriendship
                            );
                            //TODO: Consider SignalR Push
                            return success ? Results.Ok() : Results.Conflict();
                        }
                    }

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
