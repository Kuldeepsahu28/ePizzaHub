using ePizzaHub.Models;
using ePizzaHub.Services.Interfaces;
using ePizzaHub.UI.Helpers;
using ePizzaHub.UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ePizzaHub.UI.Controllers
{
    public class PaymentController : BaseController
    {
        IPaymentService _paymentService;
        IOrderService _orderService;
        IConfiguration _configuration;

        public PaymentController(IPaymentService paymentService, IConfiguration configuration, IOrderService orderService)
        {
            _paymentService = paymentService;
            _configuration = configuration;
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            PaymentModel payment = new PaymentModel();
            CartModel cart = TempData.Peek<CartModel>("CartDetails");
            if (cart != null)
            {
                payment.RazorpayKey = _configuration["RazorPay:Key"];
                payment.Cart = cart;
                payment.GrandTotal = Math.Round(cart.GrandTotal);
                payment.Currency = "INR";
                payment.Description = string.Join(",", cart.Items.Select(r => r.Name));
                payment.Receipt = Guid.NewGuid().ToString();
                payment.OrderId = _paymentService.CreateOrder(payment.GrandTotal * 100, payment.Currency, payment.Receipt);
                return View(payment);
            }
            return RedirectToAction("Index", "Cart");
        }


        public IActionResult Status(IFormCollection form)
        {
            if (form.Keys.Count > 0)
            {
                //string paymentId = form["rzp_paymentId"];
                //string orderId = form["rzp_orderId"];
                //string signature = form["rzp_signature"];
                //string transactionId = form["Receipt"];
                //string currency = form["Currency"];

                string paymentId = form["rzp_paymentid"];
                string orderId = form["rzp_orderid"];
                string signature = form["rzp_signature"];
                string transactionId = form["Receipt"];
                string currency = form["Currency"];

                bool IsSignatureValid = _paymentService.VerifySignature(signature, orderId, paymentId);
                if (IsSignatureValid)
                {
                    var payment = _paymentService.GetPaymentDetails(paymentId);
                    string status = payment["status"];
                    CartModel cart = TempData.Peek<CartModel>("CartDetails");
                    TempData.Keep("CartDetails");
                    PaymentDetailsModel model = new PaymentDetailsModel
                    {
                        CartId = cart.Id,
                        Total = cart.Total,
                        Tax = cart.Tax,
                        GrandTotal = cart.GrandTotal,
                        Currency = currency,
                        CreatedDate = DateTime.Now,
                        Status = status,

                        TransactionId = transactionId,
                        Id = paymentId,
                        Email = CurrentUser.Email,
                        UserId = CurrentUser.Id,
                    };

                    int result = _paymentService.SavePaymentDetails(model);
                    if (result > 0)
                    {
                        Response.Cookies.Delete("CartId");
                        //TempData.Remove("CartId");
                        AddressViewModel addressViewModel = TempData.Peek<AddressViewModel>("Address");

                        OrderModel order = new OrderModel
                        {
                            PaymentId = paymentId,
                            UserId = model.UserId,
                            CreatedDate = DateTime.Now,
                            Id = orderId,
                            Street = addressViewModel.Street,
                            Locality = addressViewModel.Locality,
                            City = addressViewModel.City,
                            ZipCode = addressViewModel.ZipCode,
                            PhoneNumber = addressViewModel.PhoneNumber,
                        };

                        foreach (var item in cart.Items)
                        {
                            ItemModel orderItem = new ItemModel
                            {
                                ItemId = item.ItemId,
                                UnitPrice = item.UnitPrice,
                                Quantity = item.Quantity,
                                Total = item.Total,

                            };
                            order.Items.Add(orderItem);
                        }

                        _orderService.PlaceOrder(order);

                        TempData.Set("PaymentDetails", model);

                        return RedirectToAction("Receipt");
                    }


                }
    ;
            }

            ViewBag.Message = "Your payment has been failed. You can contact us at support@epizzahub.com.";

            return View();
        }

        public IActionResult Receipt()
        {
            PaymentDetailsModel model = TempData.Peek<PaymentDetailsModel>("PaymentDetails");
            AddressViewModel addressViewModel = TempData.Peek<AddressViewModel>("Address");
            CartModel cart = TempData.Peek<CartModel>("CartDetails");
            TempData.Remove("CartId");
            ReceiptViewModel receiptData = new ReceiptViewModel
            {
                PaymentDetail = model,
                Address = addressViewModel,
                Items = cart.Items
            };
            return View(receiptData);

          
        }
    }
}
