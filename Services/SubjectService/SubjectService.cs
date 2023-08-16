using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpaBookingApp.Data;
using SpaBookingApp.Dtos.Subject;
using SpaBookingApp.Models;

namespace SpaBookingApp.Services.SubjectService
{
    public class SubjectService : ISubjectService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public SubjectService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<GetSubjectDto>> AddSubject(AddSubjectDto newSubject)
        {
            var serviceResponse = new ServiceResponse<GetSubjectDto>();
            var subject = _mapper.Map<Subject>(newSubject);

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();
            serviceResponse.Data = _mapper.Map<GetSubjectDto>(subject);

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetSubjectDto>> DeleteSubject(int id)
        {
            var serviceResponse = new ServiceResponse<GetSubjectDto>();
            try
            {
                var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == id);
                if (subject is null)
                {
                    throw new Exception($"Subject with ID '{id}' not found");
                }

                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<GetSubjectDto>(subject);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetSubjectDto>>> GetAllSubjects(int pageIndex)
        {
            var serviceResponse = new ServiceResponse<List<GetSubjectDto>>();
            var pageSize = 5;

            var totalCount = await _context.Subjects.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var subjects = await _context.Subjects
                .OrderBy(s => s.Id)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            serviceResponse.Data = subjects.Select(s => _mapper.Map<GetSubjectDto>(s)).ToList();
            serviceResponse.PageInformation = new PageInformation
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return serviceResponse;
        }


        public async Task<ServiceResponse<GetSubjectDto>> GetSubjectById(int id)
        {
            var serviceResponse = new ServiceResponse<GetSubjectDto>();
            var dbSubject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == id);
            if (dbSubject is null)
            {
                throw new Exception($"Subject with ID '{id}' not found");
            }
            serviceResponse.Data = _mapper.Map<GetSubjectDto>(dbSubject);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetSubjectDto>> UpdateSubject(UpdateSubjectDto updatedSubject)
        {
            var serviceResponse = new ServiceResponse<GetSubjectDto>();
            try
            {
                var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == updatedSubject.Id);
                if (subject is null)
                {
                    throw new Exception($"Subject with ID '{updatedSubject.Id}' not found");
                }

                _mapper.Map(updatedSubject, subject);
                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetSubjectDto>(subject);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }
    }
}
