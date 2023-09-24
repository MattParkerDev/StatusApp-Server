namespace Application.DTOs;

public class MessageDto
{
    public int? Id { get; set; }
    public string AuthorUserName { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdated { get; set; }
    public Guid ChatId { get; set; }
}
