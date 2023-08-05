using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SpaBookingApp.Data
{
    public interface IAuthRepository
    {
        Task<bool> UserExists(string email);
        
        Task<ServiceResponse<int>> Register(User user, string password, UserRole role, string phoneNumber, string confirmPassword);
        Task<ServiceResponse<bool>> VerifyAccount(string email, string verificationCode);
        Task<ServiceResponse<string>> Login(UserLoginDto request);

        Task<ServiceResponse<bool>> ChangePassword(int userId, UserChangePasswordDto changepasswordDto);
        Task<ServiceResponse<bool>> ResetPassword(string email);
        
        Task<ServiceResponse<UserProfileDto>> GetProfile(int userId);
        Task<ServiceResponse<bool>> UpdateProfile(int userId, UserProfileUpdateDto profileDto);
        Task<ServiceResponse<bool>> DeleteAccount(int userId, UserDeleteDto deleteDto);

        Task SeedAdminUser();
        Task<int> DeleteUnverifiedAccounts();
    }
}