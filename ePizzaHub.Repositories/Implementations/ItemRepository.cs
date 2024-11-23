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
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        public ItemRepository(AppDbContext db) : base(db)
        {

        }
        public IEnumerable<Item> GetItemsWithCategory()
        {
            var items = _db.Items.Include(item => item.Category).Include(i=>i.ItemType).Select(item => new Item
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                UnitPrice = item.UnitPrice,
                ImageUrl = item.ImageUrl,
                CategoryId = item.CategoryId,
                ItemTypeId = item.ItemTypeId,
                CreatedDate = item.CreatedDate,
                Category = new Category
                {
                    Id = item.Category.Id,
                    Name = item.Category.Name
                    // Do not include the Items collection here
                },
                 ItemType=new ItemType
                 {
                      Id=item.ItemType.Id,
                      Name=item.ItemType.Name
                 }
            })
     .ToList();

            //var items = _db.Items.Include(i => i.Category).Include(i=>i.ItemType).ToList();

            return items;
        }
    }
}
