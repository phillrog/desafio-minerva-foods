using AutoMapper;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Events;
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
                           opt => opt.MapFrom(src => src.DeliveryTerm != null ? src.DeliveryTerm.EstimatedDeliveryDate : (DateTime?)null))
                .ForMember(dest => dest.DeliveryDays,
                           opt => opt.MapFrom(src => src.DeliveryTerm != null ? src.DeliveryTerm.DeliveryDays : (int?)null))
                .ForMember(dest => dest.Items,
                           opt => opt.MapFrom(src => src.Items));

            CreateMap<RegisterOrderCommand, Order>()
                .ConstructUsing((src, context) =>
                {
                    var items = context.Mapper.Map<List<OrderItem>>(src.Items);
                    return new Order(src.CustomerId, src.PaymentConditionId, items);
                });

            CreateMap<OrderItemRequest, OrderItem>()
                .ConstructUsing(src => new OrderItem(src.ProductName, src.Quantity, src.UnitPrice));

        }
    }
}
