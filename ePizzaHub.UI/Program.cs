
using Microsoft.AspNetCore.Authentication.Cookies;
using ePizzaHub.Services;
using WebMarkupMin.AspNetCore8;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();






// Services
ServiceRegistration.RegisterServices(builder.Services, builder.Configuration);

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "ePizzaHub";
        options.LoginPath = "/Account/Login";
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Account/AccessDenied";
    });


// HTML Minification
builder.Services.AddWebMarkupMin(options =>
{
    options.AllowMinificationInDevelopmentEnvironment = true;
    options.AllowCompressionInDevelopmentEnvironment = true;
    options.DisablePoweredByHttpHeaders = true;
})
    .AddHtmlMinification(options =>
    {
        options.MinificationSettings.RemoveRedundantAttributes = true;
        options.MinificationSettings.MinifyInlineJsCode = true;
        options.MinificationSettings.MinifyInlineCssCode = true;
        options.MinificationSettings.MinifyEmbeddedJsonData = true;
        options.MinificationSettings.MinifyEmbeddedCssCode = true;
    })
    .AddHttpCompression();


// loging
builder.Host.UseSerilog((ctx, lc) =>
lc.ReadFrom.Configuration(ctx.Configuration));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseWebMarkupMin();
app.UseHttpsRedirection();
app.UseStaticFiles();
//app.UseStaticFiles(new StaticFileOptions
//{
//    OnPrepareResponse = ctx =>
//    {
//        //const int durationInSeconds = 60 * 60 * 24 * 7; //Secs*Mins*Hrs*Days
//        const int durationInSeconds = 60 * 10; //Secs*Mins*Hrs*Days
//        ctx.Context.Response.Headers["cache-control"] =
//        "public, max-age=" + durationInSeconds;
//    }
//});

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
