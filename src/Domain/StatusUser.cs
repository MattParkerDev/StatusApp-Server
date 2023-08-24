using System.ComponentModel.DataAnnotations;
using Domain.Common.Base;

namespace Domain;

public class StatusUser
{
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool Online { get; set; }

    public virtual ICollection<Chat> Chats { get; set; } = new HashSet<Chat>();
}
