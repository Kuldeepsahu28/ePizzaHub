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
    public class CartService : Service<Cart>, ICartService
    {
        ICartRepository _cartRepository;
        public CartService(ICartRepository cartRepository) : base(cartRepository)
        {
            _cartRepository = cartRepository;


        }

        public CartModel AddItem(int UserId, Guid CartId, int ItemId, decimal UnitPrice, int Quantity)
        {
            try
            {
                Cart cart = _cartRepository.GetCart(CartId);
                if (cart == null)
                {
                    cart = new Cart
                    {
                        Id = CartId,
                        UserId = UserId,
                        CreatedDate = DateTime.Now,
                        IsActive = true
                    };
                    CartItem cartItem = new CartItem
                    {
                        CartId = CartId,
                        ItemId = ItemId,
                        Quantity = Quantity,
                        UnitPrice = UnitPrice,

                    };
                    cart.CartItems.Add(cartItem);
                    _cartRepository.Add(cart);
                    _cartRepository.SaveChanges();
                }
                else
                {
                    CartItem cartItem = cart.CartItems.Where(i => i.ItemId == ItemId).FirstOrDefault();
                    if (cartItem == null)
                    {
                        cartItem = new CartItem
                        {
                            CartId = CartId,
                            ItemId = ItemId,
                            Quantity = Quantity,
                            UnitPrice = UnitPrice,
                        };
                        cart.CartItems.Add(cartItem);

                    }
                    else
                    {
                        cartItem.Quantity += Quantity;
                    }
                    _cartRepository.Update(cart);
                    _cartRepository.SaveChanges();
                }
                return new CartModel
                {
                    Id = cart.Id,
                    UserId = cart.UserId,
                    CreatedDate = cart.CreatedDate,
                    Items = cart.CartItems.Select(x => new ItemModel
                    {
                        Id = x.ItemId,
                        Quantity = x.Quantity,
                        UnitPrice = x.UnitPrice

                    }).ToList()
                };
            }
            catch (Exception ex)
            {

              
            }
            return null;
        }

        public int DeleteItem(Guid cartId, int ItemId)
        {
            return _cartRepository.DeleteItem(cartId, ItemId);
        }

        public int GetCartCount(Guid cartId)
        {
          var cartCount=  _cartRepository.GetCart(cartId);
            return cartCount != null ? cartCount.CartItems.Count : 0; 
        }

        public CartModel GetCartDetails(Guid cartId)
        {
            var data = _cartRepository.GetCartDetails(cartId);
            if (data != null)
            {
                CartModel cart = new CartModel();
                cart.Id = data.Id;
                cart.UserId = data.UserId;
                cart.CreatedDate = data.CreatedDate;
                cart.Items = data.ItemDetails.Select(x => new ItemModel
                {
                    Id = x.Id,
                    ItemId = x.ItemId,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    Name = x.Name,
                    Description = x.Description,
                    ImageUrl = x.ImageUrl,
                    Total = x.Total
                }).ToList();

                //GST Calculation: 5%
                cart.Total = cart.Items.Sum(x => x.Total);
                cart.Tax = Math.Round(cart.Total * 0.05m,2);
                cart.GrandTotal = cart.Total + cart.Tax;
                return cart;
            }

            return null;
        }

        public int UpdateCart(Guid CartId, int UserId)
        {
            return _cartRepository.UpdateCart(CartId, UserId);
        }

        public int UpdateQuantity(Guid CartId, int Id, int Quantity)
        {
           return _cartRepository.UpdateQuantity(CartId, Id, Quantity);
        }
    }
}
