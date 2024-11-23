using AutoMapper;
using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ePizzaHub.Services.Implementations
{
    public class ItemService : Service<Item>, IItemService
    {
        IMapper _mapper;
        IItemRepository _itemRepository;
        public ItemService(IItemRepository itemRepository, IMapper mapper) : base(itemRepository)
        {
            _mapper = mapper;
            _itemRepository = itemRepository;
        }
        public IEnumerable<ItemModel> GetItems()
        {
            var data = _repository.GetAll();

            return _mapper.Map<IEnumerable<ItemModel>>(data);
        }

        public IEnumerable<ItemAPIModel> GetItemsWithCategory()
        {
            List<ItemModel> itemList = new List<ItemModel>();
            var items = _itemRepository.GetItemsWithCategory();

           return _mapper.Map<IEnumerable<ItemAPIModel>>(items);
            //foreach (var item in items)
            //{
            //    itemList.Add(new ItemModel
            //    {
            //        Id = item.Id,
            //        Category = item.Category.Name,
            //        Name = item.Name,
            //        //ItemType = item.ItemType.Name,
            //        ItemTypeId = item.ItemTypeId,
            //        Description = item.Description,
            //        ImageUrl = item.ImageUrl,
            //        UnitPrice = item.UnitPrice,
            //        CreatedDate = item.CreatedDate,
            //        CategoryId = item.CategoryId


            //    });
            //}

           // return itemList;
        }


    }
}
