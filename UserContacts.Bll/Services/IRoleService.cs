using UserContacts.Bll.Dtos;
using UserContacts.Dal.Entities;

namespace UserContacts.Bll.Services;

public interface IRoleService
{
    Task<long> AddRoleAsync(UserRoleDto role);
    Task DeleteRoleAsync(long roleId);
    Task<ICollection<UserGetDto>> GetAllUsersByRoleAsync(string role);
    Task<List<RoleGetDto>> GetAllRolesAsync();
    Task<long> GetRoleIdAsync(string role);
}