using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpaBookingApp.Data;

namespace SpaBookingApp.Services.ContactService
{
    public class ContactService : IContactService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IEmailService _emailService;
        private readonly ISubjectService _subjectService;

        public ContactService(IMapper mapper, DataContext context, IEmailService emailService, ISubjectService subjectService)
        {
            _mapper = mapper;
            _context = context;
            _emailService = emailService;
            _subjectService = subjectService;
        }

        public async Task<ServiceResponse<ContactDto>> AddContact(ContactDto newContact)
        {
            var serviceResponse = new ServiceResponse<ContactDto>();
            try
            {
                var contact = _mapper.Map<Contact>(newContact);

                var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.Id == newContact.SubjectId);

                if (subject == null)
                {
                    throw new Exception($"Subject with ID '{newContact.SubjectId}' not found");
                }

                // Validate email format
                if (!IsValidEmail(newContact.Email))
                {
                    throw new Exception("Invalid email format");
                }

                // Validate phone format
                if (!IsValidPhone(newContact.Phone))
                {
                    throw new Exception("Invalid phone format");
                }

                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<ContactDto>(contact);

                // Asynchronously send email notifications
                var sendEmailTasks = Task.WhenAll(
                    SendContactNotificationToAdmin(contact),
                    SendContactConfirmationToUser(contact)
                );

                // Wait for email tasks to complete
                await sendEmailTasks;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }



        public async Task<ServiceResponse<ContactDto>> DeleteContact(int id)
        {
            var serviceResponse = new ServiceResponse<ContactDto>();
            try
            {
                var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id);
                if (contact is null)
                {
                    throw new Exception($"Contact with ID '{id}' not found");
                }

                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();

                serviceResponse.Data = _mapper.Map<ContactDto>(contact);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<ContactDto>>> GetAllContacts(int pageIndex)
        {
            int pageSize = 5; // Số lượng liên hệ hiển thị trên mỗi trang

            var serviceResponse = new ServiceResponse<List<ContactDto>>();
            var dbContacts = await _context.Contacts.ToListAsync();

            var allContacts = dbContacts.Select(c => _mapper.Map<ContactDto>(c)).ToList();

            // Phân trang: Lấy danh sách liên hệ cho trang hiện tại
            var pagedContacts = allContacts.Skip(pageIndex * pageSize).Take(pageSize).ToList();

            // Tạo đối tượng PageInfo và cập nhật vào serviceResponse
            var pageInfo = new PageInformation
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = allContacts.Count,
                TotalPages = (int)Math.Ceiling((double)allContacts.Count / pageSize)
            };

            serviceResponse.Success = true;
            serviceResponse.Data = pagedContacts;
            serviceResponse.PageInformation = pageInfo;

            return serviceResponse;
        }


        public async Task<ServiceResponse<ContactDto>> GetContactById(int id)
        {
            var serviceResponse = new ServiceResponse<ContactDto>();
            var dbContact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id);
            if (dbContact is null)
            {
                throw new Exception($"Contact with ID '{id}' not found");
            }
            serviceResponse.Data = _mapper.Map<ContactDto>(dbContact);
            return serviceResponse;
        }

        private bool IsValidEmail(string email)
        {
            // Regular expression for basic email validation
            string pattern = @"^[\w\.-]+@[\w\.-]+\.\w+$";
            return Regex.IsMatch(email, pattern);
        }

        private bool IsValidPhone(string phone)
        {
            // Regular expression for basic phone number validation
            string pattern = @"^\d{10}$";
            return Regex.IsMatch(phone, pattern);
        }

        private async Task SendContactNotificationToAdmin(Contact contact)
        {
            // Compose the email content for admin
            MailRequest adminMailRequest = new MailRequest
            {
                ToEmail = "thebestspa.fpt@gmail.com", // Change to your admin's email
                Subject = "New Contact Submission",
                Body = $"<p><strong>{contact.Name}</strong> has sent a contact to us with the following information:</p>" +
                    $"<p><strong>Subject:</strong><br>{(await _subjectService.GetSubjectById(contact.SubjectId))?.Data.Name}</p>" +
                    $"<p><strong>Email:</strong><br>{contact.Email}</p>" +
                    $"<p><strong>Message:</strong><br>{contact.Message}</p>" +
                    $"<p><strong>Phone:</strong><br>{contact.Phone}</p>"
            };

            // Send the email to admin
            await _emailService.SendEmailAsync(adminMailRequest);
        }

        private async Task SendContactConfirmationToUser(Contact contact)
        {
            // Compose the email content for user
            MailRequest userMailRequest = new MailRequest
            {
                ToEmail = contact.Email,
                Subject = "Contact Confirmation",
                Body = $"<p><strong>Dear {contact.Name},</strong></p>" +
                        "<p>Thank you for contacting us. We will get back to you as soon as possible.</p>" +
                        "<p>Please check your email or phone number for our response.</p>" +
                        "<p>Best regards,</p>" +
                        "<p><strong>FSpa</strong></p>"
            };

            // Send the email to user
            await _emailService.SendEmailAsync(userMailRequest);
        }

    }
}
