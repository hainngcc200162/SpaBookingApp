using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SpaBookingApp.Data
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<int>> Register(User user, string password, UserRole role, string phoneNumber);
        // Task<ServiceResponse<bool>> ConfirmPhoneNumber(int userId, string verificationCode);
        Task<ServiceResponse<string>> Login(String username, string password);
        Task<bool> UserExists(string username);
        Task<ServiceResponse<bool>> ChangePassword(string username, string oldPassword, string newPassword);

    }
}