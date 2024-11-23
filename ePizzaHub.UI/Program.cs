using ePizzaHub.Core;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using ePizzaHub.Core.Entities;
using ePizzaHub.Services.Interfaces;
using ePizzaHub.Services.Implementations;
using Microsoft.AspNetCore.Authentication.Cookies;
using ePizzaHub.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

// Services
ServiceRegistration.RegisterServices(builder.Services,builder.Configuration);





builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "ePizzaHub";
        options.LoginPath = "/Account/Login";
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapControllerRoute(
           name: "areas",
           pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
         );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
