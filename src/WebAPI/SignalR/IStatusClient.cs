using Application.DTOs;
using Domain;

namespace WebAPI.SignalR;

public interface IStatusClient
{
    // Methods that a client listens for - connection.on(...)
    Task ReceiveBroadcast(string user, string message);
    Task ReceiveMessage(MessageDto message);
    Task ReceiveUpdatedUser(StatusUserDto friend);
    Task ReceiveUpdatedFriendship(FriendshipDto friendship);
    Task DeleteFriend(string userName);
}
