using Microsoft.EntityFrameworkCore;
using StatusApp_Server.Domain;
using StatusApp_Server.Infrastructure;
using System.Runtime.CompilerServices;

namespace StatusApp_Server.Application
{
    public static class MinimalApiEndpoints
    {
        public static void RegisterMessageAPIs(this WebApplication app)
        {
            app.MapGet(
                    "/getmessages",
                    (ChatContext db, int ChatId) =>
                    {
                        var messages = db.Messages.Where(s => s.ChatId == ChatId);
                        return messages.Count() != 0 ? Results.Ok(messages) : Results.NoContent();
                    }
                )
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
                .WithName("UpdateMessage")
                .WithOpenApi();
        }

        public static void RegisterUserAPIs(this WebApplication app)
        {
            app.MapGet(
                    "/getUser",
                    (ChatContext db, int AccountId) =>
                    {
                        var user = db.Users.First(s => s.AccountId == AccountId);
                        return Results.Ok(user);
                    }
                )
                .WithName("GetUser")
                .WithOpenApi();

            app.MapGet(
                    "/getAccount",
                    (ChatContext db, int AccountId) =>
                    {
                        var account = db.Accounts.First(s => s.AccountId == AccountId);
                        return Results.Ok(account);
                    }
                )
                .WithName("GetAccount")
                .WithOpenApi();

            app.MapPut(
                    "/createUser",
                    async (ChatContext db, string FirstName, string LastName, string Email) =>
                    {
                        var user = new User();
                        var account = new Account();
                        var success = false;
                        user.FirstName = FirstName;
                        user.LastName = LastName;
                        account.Email = Email;

                        db.Users.Add(user);
                        db.Accounts.Add(account);
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
                        return success == true ? Results.Ok(user) : Results.Conflict(user);
                    }
                )
                .WithName("CreateUser")
                .WithOpenApi();

            app.MapDelete(
                    "deleteUser",
                    async (ChatContext db, int AccountId) =>
                    {
                        var targetUser = db.Users.First(s => s.AccountId == AccountId);
                        var targetAccount = db.Accounts.First(s => s.AccountId == AccountId);
                        db.Users.Remove(targetUser);
                        db.Accounts.Remove(targetAccount);
                        await db.SaveChangesAsync();
                        return Results.Ok(targetUser);
                    }
                )
                .WithName("DeleteUser")
                .WithOpenApi();

            app.MapPatch(
                    "updateUser",
                    async (
                        ChatContext db,
                        int AccountId,
                        string? FirstName,
                        string? LastName,
                        string? Email,
                        string? Password,
                        string? UserName,
                        string? PhoneNumber,
                        string? Status,
                        bool? Online
                    ) =>
                    {
                        var targetUser = db.Users.First(s => s.AccountId == AccountId);
                        var targetAccount = db.Accounts.First(s => s.AccountId == AccountId);

                        targetUser.FirstName = FirstName ?? targetUser.FirstName;
                        targetUser.LastName = LastName ?? targetUser.LastName;
                        targetAccount.Email = Email ?? targetAccount.Email;
                        targetAccount.Password = Password ?? targetAccount.Password;
                        targetUser.UserName = UserName ?? targetUser.UserName;
                        targetAccount.UserName = UserName ?? targetAccount.UserName;
                        targetAccount.PhoneNumber = PhoneNumber ?? targetAccount.PhoneNumber;
                        targetUser.Status = Status ?? targetUser.Status;
                        targetUser.Online = Online ?? targetUser.Online;

                        db.Users.Update(targetUser);
                        db.Accounts.Update(targetAccount);
                        await db.SaveChangesAsync();
                        return Results.Ok(targetUser);
                    }
                )
                .WithName("UpdateUser")
                .WithOpenApi();
        }

        public static void RegisterFriendAPIs(this WebApplication app)
        {
            app.MapGet(
                    "/getfriends",
                    (ChatContext db, int AccountId) => // Pass your AccountId here to retrieve your associated friends
                    {
                        var friendships = db.Friendships
                            .Where(s => s.AccountId == AccountId && s.AreFriends == true)
                            .ToList();
                        var friendIdList = new List<int>();
                        foreach (var item in friendships)
                        {
                            friendIdList.Add(item.FriendId);
                        }
                        //Console.WriteLine(friendIdList);
                        var friends = db.Users.Where(s => friendIdList.Contains(s.AccountId));
                        return friends.Count() != 0 ? Results.Ok(friends) : Results.NoContent();
                    }
                )
                .WithName("GetFriends")
                .WithOpenApi();

            app.MapGet(
                    "/getfriendships",
                    (ChatContext db, int AccountId, bool? AreFriends) => // Pass your AccountId here to retrieve your associated friendships
                    {
                        var friendships =
                            AreFriends == null // Optional AreFriends returns all friendships regardless of status if not supplied in request
                                ? db.Friendships.Where(s => s.AccountId == AccountId)
                                : db.Friendships.Where(
                                    s => s.AccountId == AccountId && s.AreFriends == AreFriends
                                );
                        return friendships.Count() != 0
                            ? Results.Ok(friendships)
                            : Results.NoContent();
                    }
                )
                .WithName("GetFriendships")
                .WithOpenApi();

            app.MapPut(
                    "/sendfriendrequest",
                    async (ChatContext db, int AccountId, int FriendId) =>
                    {
                        var success = false;
                        try 
                        {
                            var friendUser = db.Users.First(s => s.AccountId == FriendId);
                        }
                        catch (InvalidOperationException e)
                        {
                            var errorString = $"Error: {e.Message}";
                            return Results.Conflict();
                        }
                        var myFriendship = new Friendship
                        {
                            AccountId = AccountId,
                            FriendId = FriendId,
                            Accepted = true,
                            AreFriends = false
                        };
                        var theirFriendship = new Friendship
                        {
                            AccountId = FriendId,
                            FriendId = AccountId,
                            Accepted = false,
                            AreFriends = false
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
                        return success == true
                            ? Results.Ok(myFriendship)
                            : Results.Conflict(myFriendship);
                    }
                )
                .WithName("SendFriendRequest")
                .WithOpenApi();

            app.MapPut(
                    "/actionfriendrequest",
                    async (ChatContext db, int AccountId, int FriendId, bool Accepted) => // Pass AccountId of your friend
                    {
                        var success = false;
                        var myFriendship = db.Friendships.FirstOrDefault(
                            s => s.AccountId == AccountId && s.FriendId == FriendId
                        );
                        var theirFriendship = db.Friendships.FirstOrDefault(
                            s => s.AccountId == FriendId && s.FriendId == AccountId
                        );
                        if (myFriendship != null && theirFriendship != null)
                        {
                            if (Accepted == true)
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
                        return success == true
                            ? Results.Ok(myFriendship)
                            : Results.Conflict(myFriendship);
                    }
                )
                .WithName("ActionFriendRequest")
                .WithOpenApi();

            app.MapDelete(
                    "/removefriend",
                    async (ChatContext db, int AccountId, int FriendId) => // Pass AccountId of your friend
                    {
                        var success = false;
                        var myFriendship = db.Friendships.FirstOrDefault(
                            s => s.AccountId == AccountId && s.FriendId == FriendId
                        );
                        var theirFriendship = db.Friendships.FirstOrDefault(
                            s => s.AccountId == FriendId && s.FriendId == AccountId
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
                        return success == true
                            ? Results.Ok(myFriendship)
                            : Results.Conflict(myFriendship);
                    }
                )
                .WithName("RemoveFriend")
                .WithOpenApi();
        }
    }
}
