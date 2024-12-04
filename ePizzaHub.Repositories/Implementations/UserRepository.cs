using ePizzaHub.Core;
using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace ePizzaHub.Repositories.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext db) : base(db)
        {

        }

        public bool ChangePassword(int id, string password)
        {
            try
            {
                if (id != null && password != null)
                {
                    var user = _db.Users.Find(id);
                    if (user != null)
                    {
                        user.Password = BC.HashPassword(password);
                        _db.SaveChanges();
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {

                return false;
            }
           
           
        }

        public bool CreateUser(User user, string Role)
        {
            Role role = _db.Roles.Where(r => r.Name == Role).FirstOrDefault();
            if (role != null)
            {
                user.Roles.Add(role);
                user.Password = BC.HashPassword(user.Password);
                _db.Users.Add(user);
                _db.SaveChanges();
                return true;
            }
            return false;
        }

        public UserModel GetByEmailId(string email)
        {
            var user= _db.Users.Include(user => user.Roles).Where(e => e.Email == email).FirstOrDefault();
            if (user!=null)
            {
                return new UserModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    Roles = user.Roles.Select(r => r.Name).ToArray(),
                };
            }

            return null;
            
         
        }

        public UserModel ValidateUser(string Email, string Password)
        {
            User user = _db.Users.Include(user => user.Roles).Where(e => e.Email == Email).FirstOrDefault();


            if (user != null)
            {
                if (BC.Verify(Password, user.Password))
                {
                    UserModel userModel = new UserModel
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Phone = user.PhoneNumber,
                        Roles = user.Roles.Select(r => r.Name).ToArray(),
                    };
                    return userModel;
                }
            }
            return null;
        }
    }
}
