using ePizzaHub.Core;
using ePizzaHub.Core.Entities;
using ePizzaHub.Repositories.Implementations;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Implementations;
using ePizzaHub.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.Services
{
    public class ServiceRegistration
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")
));


            //mapper dependencies
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


            //Repository dependencies

            //services.AddScoped<IRepository<Item>, Repository<Item>>();
            services.AddScoped<IRepository<PaymentDetail>, Repository<PaymentDetail>>();
            services.AddScoped<IRepository<Category>, Repository<Category>>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();



            //Service dependencies
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IUtilityService, UtilityService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IEmailSenderService, EmailSenderService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      


        }
    }
}
