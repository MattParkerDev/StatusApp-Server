using Microsoft.EntityFrameworkCore;
using StatusApp.Server.Application.Contracts;
using StatusApp.Server.Domain;
using StatusApp.Server.Infrastructure;

namespace StatusApp.Server.Application;

public class StatusUserService : IStatusUserService
{
    private readonly StatusContext _db;

    public StatusUserService(StatusContext db)
    {
        _db = db;
    }

    public async Task<bool> CreateUserAsync(StatusUser newUser)
    {
        try
        {
            _db.StatusUsers.Add(newUser);
            await _db.SaveChangesAsync();
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }

    public async Task<bool> DeleteUserAsync(StatusUser newUser)
    {
        try
        {
            _db.StatusUsers.Remove(newUser);
            await _db.SaveChangesAsync();
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }

    public async Task<StatusUser?> GetUserByNameAsync(string userName)
    {
        try
        {
            var statusUser = await _db.StatusUsers.FirstOrDefaultAsync(s => s.UserName == userName);
            return statusUser;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> UpdateUserAsync(StatusUser newUser)
    {
        try
        {
            _db.StatusUsers.Update(newUser);
            await _db.SaveChangesAsync();
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
}
