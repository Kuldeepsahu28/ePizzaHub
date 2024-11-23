using ePizzaHub.Core;
using ePizzaHub.Core.Entities;
using ePizzaHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.Repositories.Implementations
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext db) : base(db)
        {
        }

        public Order GetOrderDetails(string orderId)
        {
            return _db.Orders.Include(o => o.OrderItems).Where(o => o.Id == orderId).FirstOrDefault();
        }

        public IEnumerable<Order> GetUserOrders(int UserId)
        {
            return _db.Orders.Include(o => o.OrderItems).Where(o => o.UserId == UserId).ToList();
        }
    }
}
