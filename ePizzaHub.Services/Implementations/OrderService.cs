using AutoMapper;
using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.Services.Implementations
{
    public class OrderService : IOrderService
    {
        IOrderRepository _orderRepository;
        IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public OrderModel GetOrderDetails(string orderId)
        {
            Order order = _orderRepository.GetOrderDetails(orderId);
            return _mapper.Map<OrderModel>(order);
        }

        public IEnumerable<OrderModel> GetUserOrders(int UserId)
        {
            var orders = _orderRepository.GetUserOrders(UserId);
            return _mapper.Map<IEnumerable<OrderModel>>(orders);
        }

        public int PlaceOrder(OrderModel model)
        {
            Order order = _mapper.Map<Order>(model);
            order.OrderItems = _mapper.Map<ICollection<OrderItem>>(model.Items);

            _orderRepository.Add(order);
            return _orderRepository.SaveChanges();
        }
    }
}
