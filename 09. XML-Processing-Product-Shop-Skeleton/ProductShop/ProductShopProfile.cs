using AutoMapper;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<UserImportModel, User>();
            this.CreateMap<ProductImportModel, Product>();
            this.CreateMap<CategoryImportModel, Category>();
            this.CreateMap<CategoryProductImportModel, CategoryProduct>();

            this.CreateMap<Product, ProductWithBuyerExportModel>()
                .ForMember(x => x.BuyerFullName, y => y.MapFrom(z => $"{z.Buyer.FirstName} {z.Buyer.LastName}"));
        }
    }
}
