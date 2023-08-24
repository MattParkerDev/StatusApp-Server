using Domain;

namespace Application.Services.Contracts;

public interface IStatusUserService
{
    public Task<bool> CreateUserAsync(StatusUser newUser);
    public Task<bool> DeleteUserAsync(StatusUser newUser);
    public Task<StatusUser?> GetUserByNameAsync(string userName);
    public Task<bool> UpdateUserAsync(StatusUser newUser);
}
