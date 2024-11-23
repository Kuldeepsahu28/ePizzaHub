using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.API.Helpers
{
    public class CustomAuthorize : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string authorization = context.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorization))
            {
                context.Result = new UnauthorizedResult();
            }
            else if (authorization.StartsWith("Bearer"))
            {
                string token = authorization.Substring(7);
                if (!string.IsNullOrEmpty(token))
                {
                    //validate token
                    var config = context.HttpContext.RequestServices.GetService<IConfiguration>();
                    string jwtKey = config.GetValue<string>("Jwt:Key");
                    string jwtIssuer = config.GetValue<string>("Jwt:Issuer");
                    string jwtAudience = config.GetValue<string>("Jwt:Audience");

                    SecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey));
                    TokenValidationParameters validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key
                    };

                    SecurityToken validatedToken;
                    JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                    var user = handler.ValidateToken(token, validationParameters, out validatedToken);
                    if (!user.Identity.IsAuthenticated)
                    {
                        context.Result=new UnauthorizedResult();
                    }
                }
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
