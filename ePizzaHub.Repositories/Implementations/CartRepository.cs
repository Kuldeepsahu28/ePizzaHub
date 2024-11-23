using ePizzaHub.Core;
using ePizzaHub.Core.DataModels;
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
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext db) : base(db)
        {
        }

        public int DeleteItem(Guid cartId, int itemId)
        {
            var cartItem = _db.CartItems.FirstOrDefault(x => x.CartId == cartId && x.ItemId == itemId);
            if (cartItem != null)
            {
                _db.CartItems.Remove(cartItem);
                return _db.SaveChanges();
            }
            return 0;
        }

        public Cart GetCart(Guid CartId)
        {
            return _db.Carts.Include(c => c.CartItems).Where(c => c.Id == CartId && c.IsActive == true).FirstOrDefault();
        }

        public CartDetails GetCartDetails(Guid CartId)
        {
            var CartDetails = (from cart in _db.Carts
                               where cart.Id == CartId && cart.IsActive == true
                               select new CartDetails
                               {
                                   Id = cart.Id,
                                   UserId = cart.UserId,
                                   CreatedDate = cart.CreatedDate,
                                   IsActive = cart.IsActive,
                                   ItemDetails = (from cartItem in _db.CartItems
                                                  join item in _db.Items on cartItem.ItemId equals item.Id
                                                  where cartItem.CartId == CartId
                                                  select new ItemDetails
                                                  {
                                                      Id = cartItem.Id,
                                                      ItemId = cartItem.ItemId,
                                                      Quantity = cartItem.Quantity,
                                                      Name = item.Name,
                                                      Description = item.Description,
                                                      UnitPrice = item.UnitPrice,
                                                      ImageUrl = item.ImageUrl,
                                                      Total = item.UnitPrice * cartItem.Quantity
                                                  }).ToList()
                               }).FirstOrDefault();
            return CartDetails;
        }

        public int UpdateCart(Guid cartId, int userId)
        {
            Cart cart = _db.Carts.Where(c => c.Id == cartId && c.IsActive == true).FirstOrDefault();
            if (cart != null)
            {
                cart.UserId = userId;
                return _db.SaveChanges();
            }
            return 0;
        }

        public int UpdateQuantity(Guid cartId, int Id, int Quantity)
        {
            var cartItem = _db.CartItems.FirstOrDefault(x => x.CartId == cartId && x.ItemId == Id);
            if (cartItem != null)
            {
                cartItem.Quantity += Quantity;
                return _db.SaveChanges();
            }

            return 0;
        }
    }
}
