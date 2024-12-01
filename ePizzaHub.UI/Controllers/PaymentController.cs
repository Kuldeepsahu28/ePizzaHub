using ePizzaHub.Models;
using ePizzaHub.Services.Interfaces;
using ePizzaHub.UI.Helpers;
using ePizzaHub.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace ePizzaHub.UI.Controllers
{
    public class PaymentController : BaseController
    {
        IPaymentService _paymentService;
        IOrderService _orderService;
        IConfiguration _configuration;
        IEmailSenderService _emailSenderService;
        IWebHostEnvironment _webHostEnvironment;

        public PaymentController(IPaymentService paymentService, IConfiguration configuration, IOrderService orderService, IEmailSenderService emailSenderService, IWebHostEnvironment webHostEnvironment)
        {
            _paymentService = paymentService;
            _configuration = configuration;
            _orderService = orderService;
            _emailSenderService = emailSenderService;
            _webHostEnvironment = webHostEnvironment;
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
            try
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


                // Sending Email
                string path = Path.Combine(_webHostEnvironment.WebRootPath, "emailTemplates", "Receipt.html");
                string htmlString = System.IO.File.ReadAllText(path);
                           
                htmlString = htmlString.Replace("{{Name}}", CurrentUser.Name);
                htmlString = htmlString.Replace("{{Address}}", addressViewModel.Street + ", " + addressViewModel.Locality + ", " + addressViewModel.City);
                htmlString = htmlString.Replace("{{Invoice Number}}", model.TransactionId);
                htmlString = htmlString.Replace("{{Date}}", model.CreatedDate.ToString("dd/MMM/yyyy"));
                htmlString = htmlString.Replace("{{SubTotal}}", "&#8377;" + model.Total.ToString());

                htmlString = htmlString.Replace("{{Tax}}", "&#8377;" + model.Tax.ToString());
                htmlString = htmlString.Replace("{{Grand Total}}", "&#8377;" + model.GrandTotal.ToString());

                string items = "";
                int sl = 1;
                foreach (var item in cart.Items)
                {
                    string i = $"  <tr>\r\n                    " +
                        $"<td style=\"padding: 10px; border-bottom: 1px solid #ddd;\">{sl}</td>\r\n  " +
                        $" <td style=\"padding: 10px; border-bottom: 1px solid #ddd;\">{item.Name}</td>\r\n" +
                        $" <td style=\"padding: 10px; border-bottom: 1px solid #ddd;\">&#8377;{item.UnitPrice}</td>\r\n" +
                        $"<td style=\"padding: 10px; border-bottom: 1px solid #ddd;\">{item.Quantity}</td>\r\n " +
                        $"<td style=\"padding: 10px; border-bottom: 1px solid #ddd;\">&#8377;{item.Total}</td>\r\n " +
                        $" </tr>";
                    items = items + i;
                    sl++;
                }
                htmlString = htmlString.Replace("{{Items}}", items);

                _emailSenderService.EmailSend(CurrentUser.Email, "Receipt", htmlString);


                return View(receiptData);
            }
            catch (Exception ex)
            {
                return View("Error", new ErrorViewModel { RequestId = ex.Message });
            }


        }
    }
}
