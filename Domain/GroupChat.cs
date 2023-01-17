using Microsoft.EntityFrameworkCore;

namespace StatusApp_Server.Domain;

[PrimaryKey(nameof(GroupId), nameof(UserName))]
public class GroupChat
{
    public int GroupId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public bool Admin { get; set; }
}
