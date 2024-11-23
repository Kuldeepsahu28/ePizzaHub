
using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Services.Implementations;
using ePizzaHub.Services.Interfaces;
using ePizzaHub.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ePizzaHub.UI.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        IItemService _itemService;
        ICartService _cartService;
      
        public HomeController(ILogger<HomeController> logger, IItemService itemService, ICartService cartService)
        {
            _logger = logger;
            _itemService = itemService;
            _cartService = cartService;

        }

        [HttpGet]
        public IActionResult Index()
        {
            var items = _itemService.GetItems();

            CartModel cart = _cartService.GetCartDetails(CartId);

            ViewBag.cartDetails = cart;

            return View(items);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
