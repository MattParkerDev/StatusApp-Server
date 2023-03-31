using Microsoft.AspNetCore.Identity;

namespace StatusApp.Server.Domain;

public class User : IdentityUser
{
    

    public Profile ToProfile()
    {
        var profile = new Profile
        {
            UserName = UserName!,
            FirstName = FirstName,
            LastName = LastName,
            Status = Status,
            Online = Online
        };
        return profile;
    }
}
