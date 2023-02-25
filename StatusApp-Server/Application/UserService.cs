using Microsoft.AspNetCore.Identity;
using StatusApp_Server.Domain;

namespace StatusApp_Server.Application;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public UserService(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<User?> GetUserByNameAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        return user;
    }

    public async Task<IdentityResult> CreateUserAsync(User newUser, string password)
    {
        var result = await _userManager.CreateAsync(newUser, password);
        return result;
    }

    public async Task UpdateUserAsync(User user)
    {
        await _userManager.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(User user)
    {
        await _userManager.DeleteAsync(user);
    }

    public async Task<SignInResult> PasswordSignInAsync(User user, string password)
    {
        var signInResult = await _signInManager.PasswordSignInAsync(user, password, true, false);
        return signInResult;
    }

    public async Task SignInAsync(User newUser)
    {
        await _signInManager.SignInAsync(newUser, isPersistent: true);
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}
