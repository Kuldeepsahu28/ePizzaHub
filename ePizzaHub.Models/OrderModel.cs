﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.Models
{
    public class OrderModel
    {
        public OrderModel()
        {
            Items = new List<ItemModel>();
        }
        public string Id { get; set; }
        public string PaymentId { get; set; }
        public int UserId { get; set; }
        public string Currency { get; set; }
        public decimal Total { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime CreatedDate { get; set; }
        public ICollection<ItemModel> Items { get; set; }
        public string Locality { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string PhoneNumber { get; set; }
    }
}
