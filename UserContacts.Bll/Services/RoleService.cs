using System.Numerics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserContacts.Bll.Dtos;
using UserContacts.Core.Errors;
using UserContacts.Dal;
using UserContacts.Dal.Entities;

namespace UserContacts.Bll.Services;

public class RoleService(MainContext _context,ILogger<RoleService> _logger) : IRoleService
{
    private RoleGetDto Converter(UserRole role)
    {
        return new RoleGetDto
        {
            Description = role.Description,
            Id = role.Id,
            Name = role.Name,
        };
    }

    private UserGetDto Converter(User user)
    {
        return new UserGetDto
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            UserId = user.UserId,
            UserName = user.UserName,
            Role = user.Role.Name,
        };
    }


    public async Task<long> AddRoleAsync(UserRoleDto role)
    {
        var roleEntity = new UserRole
        {
            Name = role.Name,
            Description = role.Description,
        };
        await _context.UserRoles.AddAsync(roleEntity);
        return roleEntity.Id;
    }

    public async Task DeleteRoleAsync(long roleId)
    {
        var role = _context.UserRoles.FirstOrDefault(x => x.Id == roleId);
        if (role != null)
        {
            throw new EntityNotFoundException(roleId.ToString());
        }

        _logger.LogInformation($"SuperAdmin Delete role {role.Name}");
        _context.UserRoles.Remove(role);
        await _context.SaveChangesAsync();
    }



    public async Task<List<RoleGetDto>> GetAllRolesAsync()
    {
        var roles = await GetAllRolesAsnc();
        return roles.Select(Converter).ToList();
    }

    public async Task<long> GetRoleIdAsync(string role) => await GetRoleIdAsnc(role);






    public async Task<ICollection<UserGetDto>> GetAllUsersByRoleAsync(string role)
    {
        var users = await GetAllUsersByRoleAsnc(role);
        return users.Select(Converter).ToList();
    }
    private async Task<List<UserRole>> GetAllRolesAsnc() => await _context.UserRoles.ToListAsync();

    private async Task<ICollection<User>> GetAllUsersByRoleAsnc(string role)
    {
        var foundRole = await _context.UserRoles.Include(u => u.Users).FirstOrDefaultAsync(_ => _.Name == role);
        if (foundRole is null)
        {
            throw new EntityNotFoundException(role);
        }
        return foundRole.Users;
    }
    private async Task<long> GetRoleIdAsnc(string role)
    {
        var foundRole = await _context.UserRoles.FirstOrDefaultAsync(_ => _.Name == role);
        if (foundRole is null)
        {
            throw new EntityNotFoundException(role + " - not found");
        }
        return foundRole.Id;
    }
}
