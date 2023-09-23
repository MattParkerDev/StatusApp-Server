using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using WebUI.Models;
using WebUI.Services;

namespace WebUI;

public class SignalRClient
{
    private readonly DataState _dataState;
    private readonly NotifierService _notifierService;
    private readonly HubConnection _connection;

    public SignalRClient(DataState dataState, NotifierService notifierService, string hubBaseUrl)
    {
        _dataState = dataState;
        _notifierService = notifierService;
        var hubUrl = $"{hubBaseUrl}/statushub";
        _connection = new HubConnectionBuilder()
            .WithUrl(
                hubUrl,
                options =>
                    options.HttpMessageHandlerFactory = innerHandler =>
                        new CookieHandler { InnerHandler = innerHandler }
            )
            .WithAutomaticReconnect()
            .Build();
        RegisterHandlers();
        _connection.Closed += async (exception) =>
        {
            if (exception == null)
            {
                Console.WriteLine("Connection closed without error.");
            }
            else
            {
                Console.WriteLine($"Connection closed due to an error: {exception}");
            }
        };
    }

    public async Task StartAsync()
    {
        await _connection.StartAsync();
    }

    public async Task StopAsync()
    {
        await _connection.StopAsync();
    }

    //TODO: Refactor to separate file
    public enum StatusHubConnectionState
    {
        Disconnected,
        Connected,
        Connecting,
        Reconnecting
    }

    public StatusHubConnectionState GetConnectionState()
    {
        StatusHubConnectionState state;
        state = _connection.State switch
        {
            HubConnectionState.Disconnected => StatusHubConnectionState.Disconnected,
            HubConnectionState.Connected => StatusHubConnectionState.Connected,
            HubConnectionState.Connecting => StatusHubConnectionState.Connecting,
            HubConnectionState.Reconnecting => StatusHubConnectionState.Reconnecting,
            _ => throw new ArgumentOutOfRangeException()
        };
        return state;
    }

    // Methods the client can call on the server
    public async Task<Message?> SendMessageAsync(Guid groupId, string data)
    {
        var message = await _connection.InvokeAsync<Message?>("SendMessage", groupId, data);
        return message;
    }

    public async Task BroadcastMessageAsync(string userName, string message)
    {
        await _connection.InvokeAsync("BroadcastMessage", userName, message);
    }

    private void RegisterHandlers()
    {
        _connection.On<string, string>(
            "ReceiveBroadcast",
            (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                Console.WriteLine(encodedMsg);
            }
        );
        _connection.On<Message>(
            "ReceiveMessage",
            async (message) =>
            {
                var chatId = message.ChatId;
                var messageList = _dataState.Chats.FirstOrDefault(s => s.Id == chatId)?.Messages;
                if (messageList != null)
                {
                    messageList.Add(message);
                }
                else
                {
                    messageList = new List<Message> { message };
                }
                await _notifierService.Update();
            }
        );

        _connection.On<StatusUserDto>(
            "ReceiveUpdatedUser",
            async (user) =>
            {
                if (user.UserName == _dataState.StatusUser?.UserName)
                    return;
                var targetUser = _dataState.Friends.FirstOrDefault(
                    s => s.UserName == user.UserName
                );
                if (targetUser is null)
                {
                    _dataState.Friends.Add(user);
                    return;
                }
                // TODO: Review regarding ordering
                _dataState.Friends.Remove(targetUser);
                _dataState.Friends.Add(user);
                await _notifierService.Update();
            }
        );
    }
}
