using Application.DTOs;
using Domain;

namespace WebAPI.SignalR;

public interface IStatusClient
{
    // Methods that a client listens for - connection.on(...)
    Task ReceiveBroadcast(string user, string message);
    Task ReceiveMessage(Message message);
    Task ReceiveUpdatedUser(StatusUserDto friend);
    Task ReceiveUpdatedFriendship(Friendship friendship);
    Task DeleteFriend(string userName);
}
