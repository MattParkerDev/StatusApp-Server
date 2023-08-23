using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Friendship
{
    [Key]
    public Guid Id { get; set; }
    public string UserName1 { get; set; } = string.Empty;
    public string UserName2 { get; set; } = string.Empty;
    public bool UserName1Accepted { get; set; } = false;
    public bool UserName2Accepted { get; set; } = false;
    public DateTime BecameFriendsDate { get; set; }
    public Guid ChatId { get; set; }
}
