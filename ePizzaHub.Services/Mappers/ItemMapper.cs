using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using AutoMapper;

namespace ePizzaHub.Services.Mappers
{
    public class ItemMapper:Profile
    {
        public ItemMapper()
        {
            CreateMap<Item, ItemModel>()
                .ForMember(dest => dest.File, opt => opt.Ignore()).ReverseMap();

            CreateMap<Item, ItemAPIModel>().ReverseMap();
               
        }
    }
}
