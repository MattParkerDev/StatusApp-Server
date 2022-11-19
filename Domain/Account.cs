using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StatusApp_Server.Domain
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
