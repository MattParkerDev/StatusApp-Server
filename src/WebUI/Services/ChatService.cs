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

    public async Task SelectChat(Friendship friendship)
    {
        if (_dataState.SelectedFriendshipForChat == friendship)
            return;
        _dataState.SelectedFriendshipForChat = friendship;

        try
        {
            _dataState.Messages[friendship.GroupId] = await _statusAppClient.GetMessagesAsync(
                friendship.GroupId
            );
        }
        catch (ApiException e)
        {
            // Unfortunately NSwag treats 204 as an error when it actually indicates success with nothing to return
            // I'm not going to redesign my api to placate NSwag so until the issue is fixed I will catch it
            if (e.StatusCode != 204)
            {
                throw;
            }
        }
        if (!_dataState.Messages.ContainsKey(_dataState.SelectedFriendshipForChat.GroupId))
        {
            _dataState.Messages[_dataState.SelectedFriendshipForChat.GroupId] = new List<Message>();
        }
    }
}
