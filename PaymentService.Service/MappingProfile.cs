using AutoMapper;
using PaymentService.Repository.Enums;
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
            CreateMap<Payment, PaymentModel>()
            .ForMember(dest => dest.PaymentMethod,
                opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
            .ForMember(dest => dest.PaymentStatus,
                opt => opt.MapFrom(src => src.PaymentStatus.ToString()));

            CreateMap<CreatePaymentRequest, Payment>()
                .ForMember(dest => dest.PaymentMethod,
                    opt => opt.MapFrom(src => (PaymentMethod)src.PaymentMethod))
                .ForMember(dest => dest.PaymentStatus,
                    opt => opt.MapFrom(_ => PaymentStatus.PENDING));
        }
    }
}
