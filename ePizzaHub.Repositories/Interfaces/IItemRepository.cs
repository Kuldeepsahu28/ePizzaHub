using ePizzaHub.Core.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.Repositories.Interfaces
{
    public interface IItemRepository:IRepository<Item>
    {
        IEnumerable<Item> GetItemsWithCategory();
    }
}
