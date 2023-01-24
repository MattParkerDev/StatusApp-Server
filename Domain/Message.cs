using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StatusApp_Server.Domain
{
    [PrimaryKey(nameof(GroupId), nameof(MessageId))]
    public class Message
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? MessageId { get; set; }
        public Guid GroupId { get; set; }
        public string AuthorUserName { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdated { get; set; }
    }
}
