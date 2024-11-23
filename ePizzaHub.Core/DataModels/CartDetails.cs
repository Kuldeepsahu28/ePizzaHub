using ePizzaHub.Core.Entities;


namespace ePizzaHub.Core.DataModels
{
    public class CartDetails:Cart
    {
        public ICollection<ItemDetails> ItemDetails { get; set; }
    }
}
