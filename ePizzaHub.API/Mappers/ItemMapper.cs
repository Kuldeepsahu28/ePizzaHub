using AutoMapper;
using ePizzaHub.API.DTOs;
using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.API.Mappers
{
    public class ItemMapper:Profile
    {
        public ItemMapper()
        {
            //Get
            CreateMap<Item, ItemDTO>().ReverseMap();

            //Add
            CreateMap<AddEditItemDTO,Item>().ReverseMap();
        }
    }
}
