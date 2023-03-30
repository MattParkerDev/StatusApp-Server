using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using StatusApp.WebUI.Models;
using StatusApp.WebUI.Services;

namespace StatusApp.WebUI;

public class SignalRClient
{
    private readonly DataState _dataState;
    private readonly IWebAssemblyHostEnvironment _hostEnvironment;
    private readonly NotifierService _notifierService;
    private readonly HubConnection _connection;

    public SignalRClient(
        DataState dataState,
        IWebAssemblyHostEnvironment hostEnvironment,
        NotifierService notifierService
    )
    {
        _dataState = dataState;
        _hostEnvironment = hostEnvironment;
        _notifierService = notifierService;
        var hubeBaseUrl = _hostEnvironment.IsDevelopment()
            ? "https://localhost:7104"
            : "https://statusapp1.azurewebsites.net";
        var hubUrl = $"{hubeBaseUrl}/statushub";
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

    public async Task SendMessageAsync(string message)
    {
        await _connection.InvokeAsync("SendMessage", message);
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
            (message) =>
            {
                var groupId = message.GroupId;
                var messageList = _dataState.Messages[groupId];
                if (messageList != null)
                {
                    messageList.Add(message);
                    return;
                }
                messageList = new List<Message> { message };
            }
        );

        _connection.On<Profile>(
            "ReceiveUpdatedUser",
            async (user) =>
            {
                if (user.UserName == _dataState.UserProfile?.UserName)
                    return;
                var targetUser = _dataState.FriendList.FirstOrDefault(
                    s => s.UserName == user.UserName
                );
                if (targetUser is null)
                {
                    _dataState.FriendList.Add(user);
                    return;
                }
                // TODO: Review regarding ordering
                _dataState.FriendList.Remove(targetUser);
                _dataState.FriendList.Add(user);
                await _notifierService.Update();
            }
        );
    }
}
