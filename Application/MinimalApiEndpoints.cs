using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using StatusApp_Server.Domain;
using StatusApp_Server.Infrastructure;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace StatusApp_Server.Application
{
    public static class MinimalApiEndpoints
    {
        public static void RegisterAuthAPIs(this WebApplication app)
        {
            app.MapGet(
                    "/checkAuth",
                    (HttpContext context) =>
                    {
                        var user = context.User.Identity.Name;
                        return user;
                    }
                )
                .RequireAuthorization()
                .WithName("CheckAuth")
                .WithOpenApi();
        }

        public static void RegisterMessageAPIs(this WebApplication app)
        {
            //TODO: Overhaul messages
            app.MapGet(
                    "/getmessages",
                    (ChatContext db, int ChatId) =>
                    {
                        var messages = db.Messages.Where(s => s.ChatId == ChatId);
                        return messages.Count() != 0 ? Results.Ok(messages) : Results.NoContent();
                    }
                )
                .RequireAuthorization()
                .WithName("GetMessages")
                .WithOpenApi();

            app.MapPut(
                    "/pushMessage",
                    async (
                        ChatContext db,
                        DateTime Created,
                        string Data,
                        int ChatId,
                        string Author
                    ) =>
                    {
                        var incomingMessage = new Message();
                        var success = false;
                        incomingMessage.Data = Data;
                        incomingMessage.ChatId = ChatId;
                        incomingMessage.Created = Created;
                        incomingMessage.AuthorId = Author;
                        db.Messages.Add(incomingMessage);
                        try
                        {
                            await db.SaveChangesAsync();
                            success = true;
                        }
                        catch (Exception e)
                        {
                            var errorString = $"Error: {e.Message}";
                            throw;
                        }

                        return success == true
                            ? Results.Ok(incomingMessage)
                            : Results.Conflict(incomingMessage);
                    }
                )
                .RequireAuthorization()
                .WithName("PushMessage")
                .WithOpenApi();
            app.MapDelete(
                    "deleteMessage",
                    async (ChatContext db, int MessageId) =>
                    {
                        var targetMessage = db.Messages.First(s => s.MessageId == MessageId);
                        db.Messages.Remove(targetMessage);
                        await db.SaveChangesAsync();
                        return Results.Ok(targetMessage);
                    }
                )
                .RequireAuthorization()
                .WithName("DeleteMessage")
                .WithOpenApi();

            app.MapPatch(
                    "updateMessage",
                    async (ChatContext db, int MessageId, string Data, DateTime LastUpdated) =>
                    {
                        var targetMessage = db.Messages.First(s => s.MessageId == MessageId);
                        targetMessage.LastUpdated = LastUpdated;
                        targetMessage.Data = Data;
                        db.Messages.Update(targetMessage);
                        await db.SaveChangesAsync();
                        return Results.Ok(targetMessage);
                    }
                )
                .RequireAuthorization()
                .WithName("UpdateMessage")
                .WithOpenApi();
        }

        public static void RegisterUserAPIs(this WebApplication app)
        {
            app.MapGet(
                    "/getUser",
                    async (ChatContext db, UserManager<User> userManager, HttpContext context) =>
                    {
                        var userName = context.User.Identity.Name;
                        User? user = await userManager.FindByNameAsync(userName);
                        if (user == null)
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
                    async (
                        ChatContext db,
                        UserManager<User> userManager,
                        SignInManager<User> signInManager
                    ) =>
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
                        ChatContext db,
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
                        ChatContext db,
                        HttpContext context,
                        UserManager<User> userManager,
                        SignInManager<User> signInManager
                    ) =>
                    {
                        var userName = context.User.Identity.Name;
                        var targetUser = await userManager.FindByNameAsync(userName);
                        if (targetUser == null)
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
                        string? firstName,
                        string? lastName,
                        string? status,
                        bool? online
                    ) =>
                    {
                        var userName = context.User.Identity.Name;
                        //TODO: Update Friendships too
                        var targetUser = await userManager.FindByNameAsync(userName);
                        if (targetUser == null)
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

                        // Push changes to user to any of their friends
                        var friendships = FriendMethods.GetFriendships(db, userName);
                        var friendUserNameList = FriendMethods.GetFriendUserNameList(friendships);
                        var usersToNotify = db.Connections
                            .Where(s => friendUserNameList.Contains(s.UserName))
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
                    async (ChatContext db, UserManager<User> userManager, HttpContext context) =>
                    {
                        var userName = context.User.Identity.Name;
                        var friends = await FriendMethods.GetFriends(db, userManager, userName);
                        return friends.Count() != 0 ? Results.Ok(friends) : Results.NoContent();
                    }
                )
                .RequireAuthorization()
                .WithName("GetFriends")
                .WithOpenApi();

            app.MapGet(
                    "/getfriendships",
                    (ChatContext db, HttpContext context, bool? areFriends) =>
                    {
                        var userName = context.User.Identity.Name;
                        var friendships =
                            areFriends == null // Optional AreFriends returns all friendships regardless of status if not supplied in request
                                ? db.Friendships.Where(s => s.UserName == userName)
                                : db.Friendships.Where(
                                    s => s.UserName == userName && s.AreFriends == areFriends
                                );
                        return friendships.Count() != 0
                            ? Results.Ok(friendships)
                            : Results.NoContent();
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
                        var userName = context.User.Identity.Name;
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
                        ChatContext db,
                        HttpContext context,
                        UserManager<User> userManager,
                        IHubContext<StatusHub, IStatusClient> hubContext,
                        string friendUserName,
                        bool accepted
                    ) =>
                    {
                        var success = false;
                        var userName = context.User.Identity.Name;
                        var user = await userManager.FindByNameAsync(userName);
                        var profile = user.ToProfile();
                        var myFriendship = db.Friendships.FirstOrDefault(
                            s => s.UserName == userName && s.FriendUserName == friendUserName
                        );
                        var theirFriendship = db.Friendships.FirstOrDefault(
                            s => s.UserName == friendUserName && s.FriendUserName == userName
                        );
                        if (myFriendship != null && theirFriendship != null)
                        {
                            if (accepted)
                            {
                                var datetime = DateTime.UtcNow;
                                myFriendship.Accepted = true;
                                myFriendship.AreFriends = true;
                                myFriendship.BecameFriendsDate = datetime;
                                theirFriendship.AreFriends = true;
                                theirFriendship.BecameFriendsDate = datetime;
                            }
                            else
                            {
                                db.Friendships.Remove(myFriendship);
                                db.Friendships.Remove(theirFriendship);
                            }
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

                        // Push this user to the new friend
                        await hubContext.Clients.User(friendUserName).ReceiveUpdatedUser(profile);
                        return Results.Ok();
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
                        UserManager<User> userManager,
                        IHubContext<StatusHub, IStatusClient> hubContext,
                        string friendUserName
                    ) =>
                    {
                        var success = false;
                        var userName = context.User.Identity.Name;
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
}
