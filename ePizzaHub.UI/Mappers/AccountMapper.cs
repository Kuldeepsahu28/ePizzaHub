using AutoMapper;
using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.UI.Mappers
{
    public class AccountMapper:Profile
    {
        public AccountMapper()
        {
            CreateMap<SignupViewModel, User>();
               
        }
    }
}
