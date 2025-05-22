using UserContacts.Dal.Entities;

namespace UserContacts.Repository.Services;

public interface IContactRepository
{
    Task<long> AddContactAsync(Contact contact);
    Task<Contact> GetContactByIdAsync(long contactId, long userId);
    Task<List<Contact>> GetAllContactsAsync(long userId);
    Task DeleteContactAsync(long contactId, long userId);
    Task UpdateContactAsync(Contact contact);
}