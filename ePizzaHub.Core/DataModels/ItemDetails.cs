using ePizzaHub.Core.Entities;


namespace ePizzaHub.Core.DataModels
{
    public class ItemDetails : Item
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}
