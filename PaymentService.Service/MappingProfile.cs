using AutoMapper;
using PaymentService.Repository.Models;
using PaymentService.Service.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Service
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<Payment, PaymentModel>().ReverseMap();
            CreateMap<CreatePaymentRequest, Payment>();
        }
    }
}
