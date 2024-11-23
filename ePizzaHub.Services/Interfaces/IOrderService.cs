using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.Services.Interfaces
{
    public interface IOrderService
    {
        OrderModel GetOrderDetails(string orderId);
        IEnumerable<OrderModel> GetUserOrders(int UserId);

        int PlaceOrder(OrderModel model);
    }
}
