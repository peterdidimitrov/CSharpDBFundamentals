namespace ProductShop;

using AutoMapper;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;

public class ProductShopProfile : Profile
{
    public ProductShopProfile()
    {
        //User
        this.CreateMap<ImportUserDto, User>();

        this.CreateMap<User, ExportUserWithAtLeastOneSoldItem>();

        //Product
        this.CreateMap<ImportProductDto, Product>();

        this.CreateMap<Product, ExportUserProductDto>();

       // this.CreateMap<Product, ExportProductInRange>()
            //.ForMember(d => d.BuyerFullName, opt => opt.MapFrom(s => s.Buyer.FirstName + " " + s.Buyer.LastName));

        //category
        this.CreateMap<ImportCategoryDto, Category>();

        //CategoryProduct
        this.CreateMap<ImportCategoryProductDto, CategoryProduct>();
    }
}
