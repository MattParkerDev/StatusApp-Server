using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StatusApp_Server.Domain
{
    public class Connection
    {
        public string UserName { get; set; } = string.Empty;

        [Key]
        public string? ConnectionId { get; set; }
    }
}
