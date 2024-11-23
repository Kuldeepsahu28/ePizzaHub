using AutoMapper;
using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.Services.Mappers
{
    public class PaymentDetailMapper : Profile
    {
        public PaymentDetailMapper()
        {
            CreateMap<PaymentDetail,PaymentDetailsModel>().ReverseMap();
        }
    }
}
