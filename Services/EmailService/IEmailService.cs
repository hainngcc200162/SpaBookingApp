using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpaBookingApp.Helpter;

namespace SpaBookingApp.Services.EmailService
{
    public interface IEmailService
    {
        Task SendEmailAsync(MailRequest mailrequest);
    }
}