using ePizzaHub.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace ePizzaHub.UI.Controllers
{
    public class BaseController : Controller
    {
       public UserModel CurrentUser
        {
            get
            {
                if (User.Claims.Count()>0)
                {
                    var userData = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData).Value;
                    return JsonSerializer.Deserialize<UserModel>(userData);
                    
                }
                return null;
            }
        }


        public Guid CartId
        {
            get
            {
                Guid id;
                string CartId = Request.Cookies["CartId"];
                if (CartId == null)
                {
                    id = Guid.NewGuid();
                    Response.Cookies.Append("CartId", id.ToString(), new CookieOptions { Expires = DateTime.Now.AddDays(1) });
                }
                else
                {
                    id = Guid.Parse(CartId);
                }
                return id;
            }
        }

    }
}
