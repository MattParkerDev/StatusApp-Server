using Microsoft.AspNetCore.Identity;
using StatusApp_Server.Domain;

namespace StatusApp_Server.Application.Contracts;

public interface IUserService
{
    Task<User?> GetUserByNameAsync(string userName);
    Task<IdentityResult> CreateUserAsync(User newUser, string password);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(User user);
    Task<SignInResult> PasswordSignInAsync(User user, string password);
    Task SignInAsync(User newUser);
    Task SignOutAsync();
}
