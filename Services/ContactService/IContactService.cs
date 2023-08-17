using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpaBookingApp.Services.ContactService
{
    public interface IContactService
    {
        Task<ServiceResponse<GetContactDto>> AddContact(AddContactDto newContact);
        Task<ServiceResponse<GetContactDto>> GetContactById(int id);
        Task<ServiceResponse<List<GetContactDto>>> GetAllContacts(int pageIndex,string searchByName, DateTime? fromDate, DateTime? toDate);
        Task<ServiceResponse<GetContactDto>>DeleteContact(int id);
        Task<ServiceResponse<GetContactDto>> UpdateContact(int id, UpdateContactDto updatedContact);
    }
}
