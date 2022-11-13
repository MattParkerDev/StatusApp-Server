using System.ComponentModel.DataAnnotations;

namespace StatusApp_Server.Domain
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        public int ChatId { get; set; }
        public string? Data { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdated { get; set; }
        public string? AuthorId { get; set; }
    }
}
