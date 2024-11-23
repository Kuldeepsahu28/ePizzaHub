using AutoMapper;
using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.UI.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.UI.Areas.Admin.Mappers
{
    public class ItemMapper:Profile
    {
        public ItemMapper()
        {
            CreateMap<AddOrEditItemVM, AddOrEditItemModel>().ReverseMap();
        }
    }
}
