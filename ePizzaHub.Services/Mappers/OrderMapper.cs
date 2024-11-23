using AutoMapper;
using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.Services.Mappers
{
    public class OrderMapper:Profile
    {
        public OrderMapper()
        {
            CreateMap<Order, OrderModel>().ReverseMap();
            CreateMap<OrderItem, ItemModel>().ReverseMap();
        }
    }
}
