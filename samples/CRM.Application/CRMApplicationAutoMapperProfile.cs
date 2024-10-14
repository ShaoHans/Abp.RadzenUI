using AutoMapper;

using CRM.Products;

namespace CRM;

public class CRMApplicationAutoMapperProfile : Profile
{
    public CRMApplicationAutoMapperProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();
    }
}
