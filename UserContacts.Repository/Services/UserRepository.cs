using Microsoft.EntityFrameworkCore;
using UserContacts.Core.Errors;
using UserContacts.Dal;
using UserContacts.Dal.Entities;

namespace UserContacts.Repository.Services;

public class UserRepository(MainContext _mainContext) : IUserRepository
{public async Task<long> InsertUserAync(User user)
    {
        await _mainContext.Users.AddAsync(user);
        await _mainContext.SaveChangesAsync();
        return user.UserId;
    }
    

    public async Task<User> SelectUserByIdAync(long id)
    {
        var user = await _mainContext.Users.Include(_=>_.Role).FirstOrDefaultAsync(x => x.UserId == id);
        if (user == null)
        {
            throw new EntityNotFoundException($"Entity with {id} not found");
        }
        return user;
    }

    public async Task<User> SelectUserByUserNameAync(string userName)
    {
        var user = await _mainContext.Users.Include(_=>_.Role).FirstOrDefaultAsync(x => x.UserName == userName);
        if (user == null)
        {
            throw new EntityNotFoundException($"Entity with {userName} not found");
        }
        return user;
    }
    public async Task UpdateUserRoleAsync(long userId, string userRole)
    {
        var user = await SelectUserByIdAync(userId);
        var role = await _mainContext.UserRoles.FirstOrDefaultAsync(x => x.Name == userRole);
        if (role == null)
        {
            throw new EntityNotFoundException($"Role : {userRole} not found");
        }
        user.RoleId = role.Id;
        _mainContext.Users.Update(user);
        await _mainContext.SaveChangesAsync();
    }

    public async Task DeleteUserByIdAsync(long userId)
    {
        var user = await SelectUserByIdAync(userId);
        _mainContext.Users.Remove(user);
        await _mainContext.SaveChangesAsync();
    }

    public Task<bool> CheckUserById(long userId) => _mainContext.Users.AnyAsync(x => x.UserId == userId);

    public Task<bool> CheckUsernameExists(string username) => _mainContext.Users.AnyAsync(_ => _.UserName == username);

    public Task<bool> CheckEmailExists(string email) => _mainContext.Users.AnyAsync(_ => _.Email == email);

    public Task<bool> CheckPhoneNumberExists(string phoneNum) => _mainContext.Users.AnyAsync(_ => _.PhoneNumber == phoneNum);
}
