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
                    "/messages",
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
                        var user = db.Users.Where(s => s.AccountId == AccountId);
                        return Results.Ok(user);
                    }
                )
                .WithName("GetUser")
                .WithOpenApi();
            app.MapPut(
                    "/createUser",
                    async (ChatContext db, string FirstName, string LastName, string Email) =>
                    {
                        var user = new User();
                        var success = false;
                        user.FirstName = FirstName;
                        user.LastName = LastName;
                        user.Email = Email;
                        db.Users.Add(user);
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
                        db.Users.Remove(targetUser);
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

                        targetUser.FirstName = FirstName ?? targetUser.FirstName;
                        targetUser.LastName = LastName ?? targetUser.LastName;
                        targetUser.Email = Email ?? targetUser.Email;
                        targetUser.Password = Password ?? targetUser.Password;
                        targetUser.UserName = UserName ?? targetUser.UserName;
                        targetUser.PhoneNumber = PhoneNumber ?? targetUser.PhoneNumber;
                        targetUser.Status = Status ?? targetUser.Status;
                        targetUser.Online = Online ?? targetUser.Online;

                        db.Users.Update(targetUser);
                        await db.SaveChangesAsync();
                        return Results.Ok(targetUser);
                    }
                )
                .WithName("UpdateUser")
                .WithOpenApi();
            ;
        }
    }
}
