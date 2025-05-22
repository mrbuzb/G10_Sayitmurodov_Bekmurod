using UserContacts.Dal.Entities;

namespace UserContacts.Repository.Services;

public interface IUserRepository
{
    Task<User> SelectUserByIdAync(long id);
    Task<User> SelectUserByUserNameAync(string userName);
    Task UpdateUserRoleAsync(long userId, string userRole);
    Task DeleteUserByIdAsync(long userId);
    Task<bool> CheckUserById(long userId);
    Task<bool> CheckUsernameExists(string username);
    Task<bool> CheckEmailExists(string email);
    Task<bool> CheckPhoneNumberExists(string phoneNum);
}