using AutoMapper;
using Warehouse_ERPIntegration.API.Models.DTO;
using Warehouse_ERPIntegration.API.Models.Entity;

namespace Warehouse_ERPIntegration.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Customer, CustomerDto>().ReverseMap();

            CreateMap<PurchaseOrderDto, PurchaseOrder>().ReverseMap();
            CreateMap<PurchaseOrderItemDto, PurchaseOrderItem>().ReverseMap();

            CreateMap<SalesOrder, SalesOrderDto>()
         .ForMember(dest => dest.CustomerExternalId, opt => opt.MapFrom(src => src.Customer.Id))
         .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<SalesOrderItem, SalesOrderItemDto>()
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.Product.ExternalProductCode));

        }
    }
}
