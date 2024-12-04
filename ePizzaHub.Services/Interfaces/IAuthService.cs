using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.Services.Interfaces
{
    public interface IAuthService:IService<User>
    {
        bool ChangePassword(int id, string password);
        bool CreateUser(User user, string Role);
        UserModel GetByEmailId(string email);
        UserModel ValidateUser(string Email, string Password);
        string GenerateForgotPasswordToken(string email);
        string ValidateForgotPasswordToken(string token);
    }
}
