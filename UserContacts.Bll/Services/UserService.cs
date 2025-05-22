using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserContacts.Core.Errors;
using UserContacts.Dal;
using UserContacts.Dal.Entities;

namespace UserContacts.Bll.Services;

public class UserService(MainContext _context,ILogger<UserService> _logger) : IUserService
{
    public async Task DeleteUserByIdAsync(long userId, string userRole)
    {
        if (userRole == "SuperAdmin")
        {
            _logger.LogInformation($"SuperAdmin Deleted user {userId}");
            await DeleteUserByIdAsnc(userId);
        }
        else if (userRole == "Admin")
        {
            var user = await SelectUserByIdAync(userId);
            if (user.Role.Name == "User")
            {
                _logger.LogInformation($"Admin Deleted user {userId}");
                await DeleteUserByIdAsnc(userId);
            }
            else
            {
                throw new NotAllowedException($"UserId : {userId} -- Admin can not delete Admin or SuperAdmin");
            }
        }
    }

    public async Task UpdateUserRoleAsync(long userId, string userRole)
    {
        await UpdateUserRoleAsnc(userId, userRole);
        _logger.LogInformation($"SuperAdmin Update user:{userId} Role To {userRole}");
    }


    private async Task<User> SelectUserByIdAync(long id)
    {
        var user = await _context.Users.Include(_ => _.Role).FirstOrDefaultAsync(x => x.UserId == id);
        if (user == null)
        {
            throw new EntityNotFoundException($"Entity with {id} not found");
        }
        return user;
    }
    private async Task UpdateUserRoleAsnc(long userId, string userRole)
    {
        var user = await SelectUserByIdAync(userId);
        var role = await _context.UserRoles.FirstOrDefaultAsync(x => x.Name == userRole);
        if (role == null)
        {
            throw new EntityNotFoundException($"UserId : {userId} -- Role : {userRole} not found");
        }
        user.RoleId = role.Id;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    private async Task DeleteUserByIdAsnc(long userId)
    {
        var user = await SelectUserByIdAync(userId);
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}
