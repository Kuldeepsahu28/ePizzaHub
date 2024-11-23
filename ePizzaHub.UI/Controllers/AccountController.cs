using AutoMapper;
using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Services.Interfaces;
using ePizzaHub.UI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace ePizzaHub.UI.Controllers
{
    public class AccountController : Controller
    {
        IAuthService _authService;
        IMapper _mapper;

        public AccountController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel vm,string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = _authService.ValidateUser(vm.Email, vm.Password);
                if (user != null)
                {
                    GenerateTicket(user);
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    if (user.Roles.Contains("Admin"))
                    {
                        TempData["success"] = "Welcome Back!";
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                    else if (user.Roles.Contains("User"))
                    {
                        TempData["success"] = "Welcome Back!";
                        return RedirectToAction("Index", "Dashboard", new { area = "User" });

                    }
                }
                else
                {
                    ModelState.AddModelError("Error", "Invalid Email or Password!");
                }
            }
            return View();
        }

        private void GenerateTicket(UserModel user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, string.Join(",",user.Roles)),
                new Claim(ClaimTypes.UserData,JsonSerializer.Serialize(user))
            };

            var identity=new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
            var principal=new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60),
            };

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,principal,properties);
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            return RedirectToAction("Index","Home");
        }  
        
        public IActionResult AccessDenied()
        {
            return View();
        }


        public IActionResult Signup()
        {          
            return View();
        }


        [HttpPost]
        public IActionResult Signup(SignupViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var mappedData = _mapper.Map<User>(vm);
                _authService.CreateUser(mappedData, "User");
                ModelState.Clear();
                return RedirectToAction("Login");
            }
            return View();
        }
    }
}
