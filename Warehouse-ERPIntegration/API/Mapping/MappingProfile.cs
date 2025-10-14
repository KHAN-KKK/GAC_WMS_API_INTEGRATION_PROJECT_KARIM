using AutoMapper;
using Warehouse_ERPIntegration.API.Models.DTO;
using Warehouse_ERPIntegration.API.Models.Entity;

namespace Warehouse_ERPIntegration.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Customer, CustomerDto>().ReverseMap();

            CreateMap<PurchaseOrderDto, PurchaseOrder>().ReverseMap();
            CreateMap<PurchaseOrderItemDto, PurchaseOrderItem>().ReverseMap();

            CreateMap<SalesOrderDto, SalesOrder>().ReverseMap();
            CreateMap<SalesOrderItemDto, SalesOrderItem>().ReverseMap();
        }
    }
}
