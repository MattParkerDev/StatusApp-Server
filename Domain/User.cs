using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace StatusApp_Server.Domain
{
    public class User
    {
        [Key]
        public int AccountId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Status { get; set; }
        public bool Online { get; set; }
    }
}
