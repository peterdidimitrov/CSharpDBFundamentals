namespace CarDealer;

using AutoMapper;
using CarDealer.DTOs.Import;
using CarDealer.Models;

public class CarDealerProfile : Profile
{
    public CarDealerProfile()
    {
        this.CreateMap<ImportSupplierDto, Supplier>();
        this.CreateMap<ImportPartDto, Part>()
            .ForMember(d => d.SupplierId, opt => opt.MapFrom(s => s.SupplierId.Value));
        this.CreateMap<ImportCarDto, Car>()
            .ForSourceMember(s => s.Parts, opt => opt.DoNotValidate());
        this.CreateMap<ImportCustomerDto, Customer>();
        this.CreateMap<ImportSaleDto, Sale>();
    }
}