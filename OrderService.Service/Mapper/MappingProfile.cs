using AutoMapper;
using OrderService.Repository.Models;
using OrderService.Service.Models;
namespace OrderService.Service.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrderMapperModel>().ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<OrderItem, OrderItemMapperModel>();
        }
    }
}
