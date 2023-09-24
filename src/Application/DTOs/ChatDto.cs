namespace Application.DTOs;

public class ChatDto
{
    public required Guid Id { get; set; }
    public required string ChatName { get; set; }
    public ICollection<StatusUserDto> ChatParticipants { get; set; } = new HashSet<StatusUserDto>();
    public ICollection<MessageDto> Messages { get; set; } = new HashSet<MessageDto>();
}
