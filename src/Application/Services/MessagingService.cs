﻿using Application.Contracts;
using Domain;

namespace Application;

public class MessagingService : IMessagingService
{
    private readonly IStatusContext _db;

    public MessagingService(IStatusContext db)
    {
        _db = db;
    }

    public List<Message> GetAllMessages(Guid chatId)
    {
        return _db.Messages.Where(s => s.ChatId == chatId).ToList();
    }

    public List<Guid> GetAllChatIdsByUserName(string userName)
    {
        var chats = _db.Chats
            // where user is a chat participant
            .Where(s => s.ChatParticipants.Any(d => d.UserName == userName))
            .Select(d => d.Id)
            .ToList();

        return chats;
    }

    public List<Chat> GetAllChatsByUserName(string userName)
    {
        var chats = _db.Chats
            // where user is a chat participant
            .Where(s => s.ChatParticipants.Any(d => d.UserName == userName))
            .ToList();

        return chats;
    }

    public async Task<Message?> CreateMessageAsUserInGroup(
        string userName,
        Guid chatId,
        string data
    )
    {
        var message = new Message
        {
            ChatId = chatId,
            Data = data,
            AuthorUserName = userName
        };
        _db.Messages.Add(message);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch
        {
            return null;
        }

        return message;
    }

    public async Task<Chat?> CreateChatForUsers(List<StatusUser> users)
    {
        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            ChatName = "New Group Chat",
            ChatParticipants = users
        };
        _db.Chats.Add(chat);
        try
        {
            await _db.SaveChangesAsync();
        }
        catch
        {
            return null;
        }

        return chat;
    }
}
