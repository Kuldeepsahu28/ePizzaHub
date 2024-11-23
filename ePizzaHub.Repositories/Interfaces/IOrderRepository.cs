using ePizzaHub.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.Repositories.Interfaces
{
    public interface IOrderRepository:IRepository<Order>
    {
        Order GetOrderDetails(string orderId);
        IEnumerable<Order> GetUserOrders(int UserId);
    }
}
