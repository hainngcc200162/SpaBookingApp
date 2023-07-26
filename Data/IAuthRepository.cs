using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SpaBookingApp.Data
{
    public interface IAuthRepository
    {
        
        Task<ServiceResponse<int>> Register(User user, string password, UserRole role, string phoneNumber, string confirmPassword);
        Task<ServiceResponse<string>> Login(String email, string password);
        Task<bool> UserExists(string email);
        Task<ServiceResponse<bool>> ChangePassword(string email, string oldPassword, string newPassword, string ConfirmNewPassword);
        Task<ServiceResponse<bool>> ResetPassword(string email);
        Task SeedAdminUser();
        
    }
}