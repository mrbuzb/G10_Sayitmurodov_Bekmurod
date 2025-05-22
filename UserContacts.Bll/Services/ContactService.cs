using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UserContacts.Bll.Dtos;
using UserContacts.Core.Errors;
using UserContacts.Dal;
using UserContacts.Dal.Entities;

namespace UserContacts.Bll.Services;

public class ContactService(MainContext _context, IValidator<ContactCreateDto> _createDtoValidator, IValidator<ContactDto> _updateDtoValidator) : IContactService
{
    private Contact Converter(ContactCreateDto contactCreateDto)
    {
        return new Contact
        {
            Address = contactCreateDto.Address,
            Email = contactCreateDto.Email,
            FirstName = contactCreateDto.FirstName,
            LastName = contactCreateDto.LastName,
            PhoneNumber = contactCreateDto.PhoneNumber,
        };
    }
    private ContactDto Converter(Contact contact)
    {
        return new ContactDto
        {
            Address = contact.Address,
            Email = contact.Email,
            FirstName = contact.FirstName,
            Id = contact.Id,
            PhoneNumber = contact.PhoneNumber,
            LastName = contact.LastName,


        };
    }
    public async Task<long> AddContactAsync(ContactCreateDto contactCreateDto, long userId)
    {
        var res = _createDtoValidator.Validate(contactCreateDto);
        if (!res.IsValid)
        {
            string errorMessages = string.Join("; ", res.Errors.Select(e => e.ErrorMessage));
            throw new NotAllowedException($"UserId : {userId} -- {errorMessages}");
        }
        var contactEntity = Converter(contactCreateDto);
        contactEntity.UserId = userId;
        contactEntity.CreatedAt = DateTime.UtcNow;
        return await AddContactAsync(contactEntity);
    }

    private async Task<long> AddContactAsync(Contact contact)
    {
        await _context.Contacts.AddAsync(contact);
        await _context.SaveChangesAsync();
        return contact.Id;
    }

    private async Task<Contact> GetContactByIdAsnc(long contactId, long userId)
    {
        var contact = await _context.Contacts.FirstOrDefaultAsync(x => x.Id == contactId);
        if (contact.UserId != userId)
        {
            throw new ForbiddenException($"User id {userId} not allowed");
        }
        return contact;
    }

    private async Task DeleteContactAsnc(long contactId, long userId)
    {
        var contact = await GetContactByIdAsnc(contactId, userId);
        _context.Contacts.Remove(contact);
        await _context.SaveChangesAsync();
    }
    private async Task<List<Contact>> GetAllContactsAsnc(long userId) => await _context.Contacts.Where(_ => _.UserId == userId).ToListAsync();

    private async Task UpdateContactAsnc(Contact contact)
    {
        _context.Contacts.Update(contact);
        await _context.SaveChangesAsync();
    }


    public async Task DeleteContactAsync(long contactId, long userId) => await DeleteContactAsnc(contactId, userId);



    public async Task<List<ContactDto>> GetAllContactsAsync(long userId)
    {
        var contacts = await GetAllContactsAsnc(userId);
        return contacts.Select(_ => Converter(_)).ToList();
    }

    public async Task<ContactDto> GetContactByIdAsync(long contactId, long userId) => Converter(await GetContactByIdAsnc(contactId, userId));
    public async Task UpdateContactAsync(ContactDto contactDto, long userId)

    {
        var res = _updateDtoValidator.Validate(contactDto);
        if (!res.IsValid)
        {
            string errorMessages = string.Join("; ", res.Errors.Select(e => e.ErrorMessage));
            throw new NotAllowedException(errorMessages);
        }
        var contact = await GetContactByIdAsnc(contactDto.Id, userId);
        contact.Email = contactDto.Email;
        contact.FirstName = contactDto.FirstName;
        contact.LastName = contactDto.LastName;
        contact.PhoneNumber = contactDto.PhoneNumber;
        contact.Address = contactDto.Address;
        await UpdateContactAsnc(contact);
    }
}
