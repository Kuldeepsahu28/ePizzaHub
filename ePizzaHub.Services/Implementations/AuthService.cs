using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace ePizzaHub.Services.Implementations
{
    public class AuthService : Service<User>, IAuthService
    {
        IUserRepository _userRepository;
        IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration) : base(userRepository)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

       

        public bool CreateUser(User user, string Role)
        {
           user.CreatedDate = DateTime.Now;
          return _userRepository.CreateUser(user, Role);
        }

        public UserModel ValidateUser(string Email, string Password)
        {
            UserModel model= _userRepository.ValidateUser(Email, Password);
            if (model!=null)
            {
                model.Token = GenerateJsonWebToken(model);
            }
            return model;
        }

        private string GenerateJsonWebToken(UserModel model)
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials=new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,model.Name),
                new Claim(JwtRegisteredClaimNames.Email,model.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public bool ChangePassword(int id, string password)
        {
            return _userRepository.ChangePassword(id, password);
         
        }

        public UserModel GetByEmailId(string email)
        {
            return _userRepository.GetByEmailId(email);
        }


        public string GenerateForgotPasswordToken(string email)
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                
                new Claim(JwtRegisteredClaimNames.Email,email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public string ValidateForgotPasswordToken(string token)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = securityKey,
                    ClockSkew = TimeSpan.Zero // Optional: no clock skew for token expiration
                }, out SecurityToken validatedToken);

                // Retrieve the email claim
           return principal.FindFirst(ClaimTypes.Email)?.Value ??
               principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value;
            }
            catch (Exception ex)
            {
                // Token validation failed
                return null;
            }
        }
    }
}