using System.Collections.Generic;
using System.Threading.Tasks;
using SpaBookingApp.Dtos.Subject;

namespace SpaBookingApp.Services.SubjectService
{
    public interface ISubjectService
    {
        Task<ServiceResponse<List<GetSubjectDto>>> GetAllSubjects();
        Task<ServiceResponse<GetSubjectDto>> GetSubjectById(int id);
        Task<ServiceResponse<GetSubjectDto>> AddSubject(AddSubjectDto newSubject);
        Task<ServiceResponse<GetSubjectDto>> UpdateSubject(UpdateSubjectDto updatedSubject);
        Task<ServiceResponse<GetSubjectDto>> DeleteSubject(int id);
    }
}
