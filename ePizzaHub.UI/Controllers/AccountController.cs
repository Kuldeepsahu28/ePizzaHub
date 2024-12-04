using AutoMapper;
using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Services.Interfaces;
using ePizzaHub.UI.Helpers;
using ePizzaHub.UI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace ePizzaHub.UI.Controllers
{
    public class AccountController : BaseController
    {
        IAuthService _authService;
        IMapper _mapper;
        IEmailSenderService _emailSenderService;
        private IWebHostEnvironment _webHostEnvironment;


        public AccountController(IAuthService authService, IMapper mapper, IEmailSenderService emailSenderService, IWebHostEnvironment webHostEnvironment)
        {
            _authService = authService;
            _mapper = mapper;
            _emailSenderService = emailSenderService;

            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel vm, string? returnUrl)
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

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60),
            };

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            return RedirectToAction("Index", "Home");
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
                var user = _authService.GetByEmailId(vm.Email);
                if (user==null)
                {
                    var mappedData = _mapper.Map<User>(vm);
                    _authService.CreateUser(mappedData, "User");
                    ModelState.Clear();
                    TempData["warning"] = "You are registered successfully";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["warning"] = "You are already registered!";
                    return RedirectToAction("Login");
                }

            }
            return View();
        }


        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {

            if (email != null)
            {
                var user = _authService.GetByEmailId(email);
                if (user != null)
                {
                    string token = _authService.GenerateForgotPasswordToken(email);
                    var resetLink = Url.Action("ChangePassword", "Account", new { token = token }, protocol: Request.Scheme);

                    string path = Path.Combine(_webHostEnvironment.WebRootPath, "emailTemplates", "ForgotPassword.html");
                    string htmlString = System.IO.File.ReadAllText(path);

                    htmlString = htmlString.Replace("{{reset_link}}", resetLink);
                    _emailSenderService.EmailSend(email, "Reset Password", htmlString);

                    ViewData["sentLink"] = "A reset password link has been sent to your email address.";

                    return View();
                }
                else
                {
                    TempData["error"] = "Something went wrong";
                }

            }
            else
            {
                ViewData["fwEmailError"] = "Please enter Email!";
                return View();
            }

            return View();
        }

       
        
        public IActionResult ChangePassword(string? token)
        {
            if (CurrentUser !=null)
            {
                return View();
            }
            if (token!=null)
            {
                var userEmail = _authService.ValidateForgotPasswordToken(token);
                if (userEmail!=null)
                {
                    return View(new ChangePasswordVM { Email = userEmail });
                }
            }
            TempData["error"] = "Something went wrong!";
            return RedirectToAction("Index", "Home");

        }


        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordVM vm)
        {
            if (vm.Email==null)
            {
                ModelState.Remove("Email");
            }
            if (vm.Email!=null)
            {
                var user = _authService.GetByEmailId(vm.Email);
                bool status = _authService.ChangePassword(user.Id, vm.Password);
                return RedirectToAction("Login");
            }
            else if (ModelState.IsValid)
            {
                bool status = _authService.ChangePassword(CurrentUser.Id, vm.Password);
                if (status)
                {
                    TempData["success"] = "Password Changed Successfully.";
                }
                else
                {
                    TempData["error"] = "Something went wrong,Try after sometime!";
                }
                if (CurrentUser.Roles.Contains("Admin"))
                {

                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                else
                {

                    return RedirectToAction("Index", "Dashboard", new { area = "User" });

                }
            }
            else
            {
                return View();
            }
           
        }
    }
}
