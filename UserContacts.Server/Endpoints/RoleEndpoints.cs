using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserContacts.Bll.Dtos;
using UserContacts.Bll.Services;

namespace UserContacts.Server.Endpoints;

public static class RoleEndpoints
{
    public static void MapRoleEndpoints(this WebApplication app)
    {
        var userGroup = app.MapGroup("/api/role")
            .RequireAuthorization()
            .WithTags("UserRole Management");

        userGroup.MapGet("/get-all-roles", [Authorize(Roles = "Admin, SuperAdmin")]
        async (IRoleService _roleService) =>
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Results.Ok(roles);
        })
        .WithName("GetAllRoles");

        userGroup.MapPost("/add-role", [Authorize(Roles = "SuperAdmin")]
        async (UserRoleDto roleDto,IRoleService _roleService) =>
        {
            var roleId = await _roleService.AddRoleAsync(roleDto);
            return Results.Ok(roleId);
        })
        .WithName("AddRole");

        userGroup.MapDelete("/delete-role", [Authorize(Roles = "SuperAdmin")]
        async (long roleId, IRoleService _roleService) =>
        {
            await _roleService.DeleteRoleAsync(roleId);
            return Results.Ok(roleId);
        })
        .WithName("DeleteRole");
    }
}
