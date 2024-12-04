using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace ePizzaHub.UI.Helpers
{
    public class CustomAuthorize : Attribute, IAuthorizationFilter
    {
        public string Roles { get; set; }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {

                if (!string.IsNullOrEmpty(Roles))
                {
                    if (!context.HttpContext.User.IsInRole(Roles))
                    {
                        context.Result = new RedirectToActionResult("AccessDenied", "Account", new { area = "" });
                    }
                }
            }
            else
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl = context.HttpContext.Request.Path, area = "" });
            }
        }
    }
}
