using ePizzaHub.Core.Entities;
using ePizzaHub.Models;


namespace ePizzaHub.Repositories.Interfaces
{
    public interface IUserRepository:IRepository<User>
    {
        bool ChangePassword(int id, string password);
        bool CreateUser(User user, string Role);
        UserModel GetByEmailId(string email);
        UserModel ValidateUser(string Email, string Password); 

    }
}
