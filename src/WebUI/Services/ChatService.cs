using WebUI.Models;

namespace WebUI.Services;

public class ChatService
{
    private readonly StatusAppClient _statusAppClient;
    private readonly DataState _dataState;

    public ChatService(StatusAppClient statusAppClient, DataState dataState)
    {
        _statusAppClient = statusAppClient;
        _dataState = dataState;
    }

    public async Task PopulateMessagesForChat(Chat chat)
    {
        try
        {
            var messages = (await _statusAppClient.GetMessagesAsync(chat.Id)).ToList();
            chat.Messages = messages;
        }
        catch (ApiException e) when (e.StatusCode == 204)
        {
            // Unfortunately NSwag treats 204 as an error when it actually indicates success with nothing to return
            // I'm not going to redesign my api to placate NSwag so until the issue is fixed I will catch it
        }
    }
}
