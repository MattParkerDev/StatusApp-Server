namespace Application.DTOs;

public class FriendshipDto
{
    public Guid Id { get; set; }
    public bool UserName1Accepted { get; set; } = false;
    public bool UserName2Accepted { get; set; } = false;
    public DateTime BecameFriendsDate { get; set; }

    // Foreign keys
    public Guid? ChatId { get; set; } = null!;
    public string UserName1 { get; set; } = string.Empty;
    public string UserName2 { get; set; } = string.Empty;
}
