using System.ComponentModel.DataAnnotations;

namespace StatusApp_Server.Domain
{
    public class Connection
    {
        public string UserName { get; set; } = string.Empty;

        [Key]
        public string? ConnectionId { get; set; }
    }
}
