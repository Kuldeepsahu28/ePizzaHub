using AutoMapper;
using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;

namespace ePizzaHub.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
       IConfiguration _configuration;
        IRepository<PaymentDetail> _paymentRepository;
        ICartRepository _cartRepository;        
        IMapper _mapper;
        readonly RazorpayClient _razorpayClient;

        public PaymentService(IConfiguration configuration, IRepository<PaymentDetail> paymentRepository, ICartRepository cartRepository, IMapper mapper)
        {
            _configuration = configuration;
            _paymentRepository = paymentRepository;
            _cartRepository = cartRepository;
            _mapper = mapper;
            _razorpayClient = new RazorpayClient(_configuration["RazorPay:Key"], _configuration["RazorPay:Secret"]);
        }

        public string CreateOrder(decimal amount, string currency, string receipt)
        {
            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount", amount); // this amount should be same as transaction amount
            input.Add("currency", currency);
            input.Add("receipt",receipt);
            Razorpay.Api.Order order = _razorpayClient.Order.Create(input);
            string orderId = order["id"].ToString();
            return orderId;
        }

        public Payment GetPaymentDetails(string paymentId)
        {
            return _razorpayClient.Payment.Fetch(paymentId);
        }



        public bool VerifySignature(string signature, string orderId, string paymentId)
        {
            string payload = string.Format("{0}|{1}", orderId, paymentId);
            string secret = RazorpayClient.Secret;
            string actualSignature = getActualSignature(payload, secret);
            return actualSignature.Equals(signature);
        }

        private static string getActualSignature(string payload, string secret)
        {
            byte[] secretBytes = StringEncode(secret);
            HMACSHA256 hashHmac = new HMACSHA256(secretBytes);
            var bytes = StringEncode(payload);

            return HashEncode(hashHmac.ComputeHash(bytes));
        }

        private static byte[] StringEncode(string text)
        {
            var encoding = new ASCIIEncoding();
            return encoding.GetBytes(text);
        }

        private static string HashEncode(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }




        // Our Own Method

        public int SavePaymentDetails(PaymentDetailsModel model)
        {
            PaymentDetail paymentDetail=_mapper.Map<PaymentDetail>(model);
            _paymentRepository.Add(paymentDetail);

            //make cart inactive
            Cart cart = _cartRepository.GetCart(model.CartId);
            cart.IsActive = false;
            return _cartRepository.SaveChanges();
        }
    }
}
