using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StatusApp_Server.Domain
{
    public class Connection
    {
        [Key]
        public int AccountId { get; set; }
        public string? ConnectionId { get; set; }
    }
}
