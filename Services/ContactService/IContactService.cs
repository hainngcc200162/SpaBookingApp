using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Services.ContactService
{
    public interface IContactService
    {
        Task<ServiceResponse<ContactDto>> AddContact(ContactDto newContact);
        Task<ServiceResponse<ContactDto>> GetContactById(int id);
        Task<ServiceResponse<List<ContactDto>>> GetAllContacts(int pageIndex);
        Task<ServiceResponse<ContactDto>> DeleteContact(int id);
    }
}
