using ePizzaHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.UI.Models
{
    public class ReceiptViewModel
    {
        public PaymentDetailsModel PaymentDetail { get; set; }
        public AddressViewModel Address { get; set; }
        public IEnumerable<ItemModel> Items { get; set; }
    }
}
