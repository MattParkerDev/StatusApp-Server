using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace StatusApp_Server.Domain;

public class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Status { get; set; } = "";
    public bool Online { get; set; }
}
