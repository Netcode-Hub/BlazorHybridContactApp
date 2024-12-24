using BlazorHybridContactApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BlazorHybridContactApp.Service
{
    public interface IContact
    {
        Task<List<MyContact>> GetAll();
        Task<MyContact?> SelectContact();
    }

    public class ContactService : IContact
    {
        public async Task<List<MyContact>> GetAll()
        {
            bool permission = await IsGranted();
            if (!permission)
                throw new UnauthorizedAccessException("You are not allowed");

            var allContacts = await Microsoft.Maui.ApplicationModel.Communication.Contacts.Default
                .GetAllAsync();
            var contactList = allContacts.Select(x => new MyContact
            {
                Email = x.Emails.FirstOrDefault()!.EmailAddress,
                PhoneNumber = x.Phones.FirstOrDefault()!.PhoneNumber,
                Id = x.Id,
                Name = x.GivenName
            }).ToList();
            return contactList;
        }

        public async Task<MyContact?> SelectContact()
        {
            bool permission = await IsGranted();
            if (!permission)
                throw new UnauthorizedAccessException("You are not allowed");

            var contact = await Microsoft.Maui.ApplicationModel.Communication.Contacts.Default
                .PickContactAsync();
            return contact is null ? null : new MyContact
            {
                Email = contact.Emails.FirstOrDefault()!.EmailAddress,
                PhoneNumber = contact.Phones.FirstOrDefault()!.PhoneNumber,
                Id = contact.Id,
                Name = contact.GivenName
            };
        }

        private async Task<bool> IsGranted()
        {
            var permission = await Permissions.RequestAsync<Permissions.ContactsRead>();
            return permission == PermissionStatus.Granted ? true : false;
        }
    }


}
