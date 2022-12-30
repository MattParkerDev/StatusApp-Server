using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StatusApp_Server.Domain
{
    public class Connection
    {
        [Key]
        public string UserName { get; set; } = string.Empty;
        public string? ConnectionId { get; set; }
    }
}
