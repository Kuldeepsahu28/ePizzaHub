using ePizzaHub.Models;
using ePizzaHub.Services.Interfaces;
using ePizzaHub.UI.Helpers;
using ePizzaHub.UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ePizzaHub.UI.Controllers
{
    public class CartController : BaseController
    {

        ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        //public Guid CartId
        //{
        //    get
        //    {
        //        Guid id;
        //        string CartId = Request.Cookies["CartId"];
        //        if (CartId == null)
        //        {
        //            id = Guid.NewGuid();
        //            Response.Cookies.Append("CartId", id.ToString(), new CookieOptions { Expires = DateTime.Now.AddDays(1) });
        //        }
        //        else
        //        {
        //            id = Guid.Parse(CartId);
        //        }
        //        return id;
        //    }
        //}

        public IActionResult Index()
        {
            CartModel cart = _cartService.GetCartDetails(CartId);
            return View(cart);
        }

        public IActionResult GetCartDetails()
        {
            CartModel cart = _cartService.GetCartDetails(CartId);
            return Json(cart);
        }

        [Route("Cart/AddToCart/{ItemId}/{UnitPrice}/{Quantity}")]
        public IActionResult AddToCart(int ItemId, decimal UnitPrice, int Quantity)
        {
            int UserId = CurrentUser != null ? CurrentUser.Id : 0;
            CartModel cart = _cartService.AddItem(UserId, CartId, ItemId, UnitPrice, Quantity);
            if (cart != null)
            {
                return Json(new { status = "success", count = cart.Items.Count });
            }
            return Json(new { status = "failed", count = 0 });
        }

        public IActionResult GetCartCount()
        {
            int cartCount = _cartService.GetCartCount(CartId);
            return Json(cartCount);
        }

        [Route("Cart/UpdateQuantity/{ItemId}/{Quantity}")]
        public IActionResult UpdateQuantity(int ItemId, int Quantity)
        {
            int count = _cartService.UpdateQuantity(CartId, ItemId, Quantity);
            return Json(count);
        }

        [Route("Cart/DeleteItem/{ItemId}")]
        public IActionResult DeleteItem(int ItemId)
        {
            int count = _cartService.DeleteItem(CartId, ItemId);
            return Json(count);
        }

        public IActionResult CheckOut()
        {
            CartModel cart = _cartService.GetCartDetails(CartId);
            if (cart.Items.Count!=0)
            {
                return View();
            }
            else
            {
                TempData["warning"] = "Please add some Items!";
                return RedirectToAction("Index", "Home");

            }
           
        }

        [HttpPost]
        public IActionResult CheckOut(AddressViewModel model)
        {
            if (ModelState.IsValid && CartId != null && CurrentUser != null)
            {
                CartModel cart = _cartService.GetCartDetails(CartId);
                cart.UserId = CurrentUser.Id;
                _cartService.UpdateCart(CartId, CurrentUser.Id);

                TempData.Set("Address", model);
                TempData.Set("CartDetails", cart);
                return RedirectToAction("Index", "Payment");
            }
            return View(model);
        }
    }
}
