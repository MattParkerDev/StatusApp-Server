using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StatusApp_Server.Domain
{
    public class Profile
    {
        [Key]
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Status { get; set; } = "";
        public bool Online { get; set; }
    }
}
