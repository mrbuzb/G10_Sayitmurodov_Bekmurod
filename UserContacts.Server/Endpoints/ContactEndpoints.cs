using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using UserContacts.Bll.Dtos;
using UserContacts.Bll.Services;
using UserContacts.Core.Errors;

namespace UserContacts.Server.Endpoints;

public static class ContactEndpoints
{
    public static void MapContactEndpoints(this WebApplication app)
    {
        var userGroup = app.MapGroup("/api/contact")
            .RequireAuthorization()
            .WithTags("Contact Management");

        userGroup.MapPost("/add-contact", [Authorize]
        async (ContactCreateDto contactCreateDto,HttpContext context, IContactService contactService) =>
        {
            var userId = context.User.FindFirst("UserId")?.Value;
            if (userId is null)
            {
                throw new ForbiddenException();
            }
            return Results.Ok(await contactService.AddContactAsync(contactCreateDto, long.Parse(userId)));
        })
            .WithName("AddContact");

        userGroup.MapDelete("/delete-contact", [Authorize]
        async (long contactId, HttpContext context, IContactService _service) =>
        {
            var userId = context.User.FindFirst("UserId")?.Value;
            if (userId is null)
            {
                throw new ForbiddenException();
            }
            await _service.DeleteContactAsync(contactId,long.Parse(userId));
            return Results.Ok();
        })
            .WithName("DeleteContact");

        userGroup.MapPut("/update-contact", [Authorize]
        async (ContactDto contact, HttpContext context, IContactService _service) =>
        {
            var userId = context.User.FindFirst("UserId")?.Value;
            if (userId is null)
            {
                throw new ForbiddenException();
            }
            await _service.UpdateContactAsync(contact,long.Parse(userId));
            return Results.Ok();
        })
            .WithName("UpdateContact");

        userGroup.MapGet("/get-contact-by-id", [Authorize]
        async (long contactId, HttpContext context, IContactService _service) =>
        {
            var userId = context.User.FindFirst("UserId")?.Value;
            if (userId is null)
            {
                throw new ForbiddenException();
            }
            var res = await _service.GetContactByIdAsync(contactId,long.Parse(userId));
            return Results.Ok(res);
        })
            .WithName("GetContactById");

        userGroup.MapGet("/get-all-contacts", [Authorize]
        async (HttpContext context, IContactService _service) =>
        {
            var userId = context.User.FindFirst("UserId")?.Value;
            if (userId is null)
            {
                throw new ForbiddenException();
            }
            var res = await _service.GetAllContactsAsync(long.Parse(userId));
            return Results.Ok(res);
        })
            .WithName("GetAllContacts");

    }
}
