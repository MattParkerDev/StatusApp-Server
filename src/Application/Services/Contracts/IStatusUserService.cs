using Domain;

namespace Application.Contracts;

public interface IStatusUserService
{
    public Task<bool> CreateUserAsync(StatusUser newUser);
    public Task<bool> DeleteUserAsync(StatusUser newUser);
    public Task<StatusUser?> GetUserByNameAsync(string userName);
    public Task<bool> UpdateUserAsync(StatusUser newUser);
}
