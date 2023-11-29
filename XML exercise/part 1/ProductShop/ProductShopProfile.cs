using AutoMapper;
using ProductShop.DTOs.Export;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            //CreateMap<User, ExportUserDTO>()
            //   .IgnoreAllPropertiesWithAnInaccessibleSetter();
            //CreateMap<Product, ExportSoldProductsDTO>();

            //CreateMap<Category, ExportedCategory>()
            //    .ForMember(x => x.Count, y => y.MapFrom(z => z.CategoryProducts.Count))
            //    .ForMember(x=>x.TotalRevenue,y=>y.MapFrom(z=>z.CategoryProducts.Sum(c=>c.Product.Price)))
            //    .ForMember(x=>x.AveragePrice, y=>y.MapFrom(z=>z.CategoryProducts.Average(c=>c.Product.Price)));

           
        }
    }
}
