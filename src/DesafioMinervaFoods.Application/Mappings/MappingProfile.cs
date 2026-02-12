using AutoMapper;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Domain.Entities;

namespace DesafioMinervaFoods.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<OrderItem, OrderItemResponse>();

            // Mapeamento do Pedido
            CreateMap<Order, OrderResponse>()
                .ForMember(dest => dest.EstimatedDeliveryDate,
                           opt => opt.MapFrom(src => src.DeliveryTerm.EstimatedDeliveryDate))
                .ForMember(dest => dest.DeliveryDays,
                           opt => opt.MapFrom(src => src.DeliveryTerm.DeliveryDays))
                .ForMember(dest => dest.Items,
                           opt => opt.MapFrom(src => src.Items));
        }
    }
}
