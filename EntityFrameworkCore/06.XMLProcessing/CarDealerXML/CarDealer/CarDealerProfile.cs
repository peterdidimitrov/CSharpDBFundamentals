namespace CarDealer;

using AutoMapper;

using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;

public class CarDealerProfile : Profile
{
    public CarDealerProfile()
    {
        //Supplier
        this.CreateMap<ImportSupplierDto, Supplier>();
        this.CreateMap<Supplier, ExportLocalSupplierDto>()
            .ForMember(d => d.PartsCount, opt => opt.MapFrom(s => s.Parts.Count));

        //Part
        this.CreateMap<ImportPartDto, Part>()
            .ForMember(d => d.SupplierId, opt => opt.MapFrom(s => s.SupplierId.Value));
        this.CreateMap<Part, ExportCarPartDto>();

        //Car
        this.CreateMap<ImportCarDto, Car>()
            .ForSourceMember(s => s.Parts, opt => opt.DoNotValidate());
        this.CreateMap<Car, ExportCarDto>();
        this.CreateMap<Car, ExportCarWithIdDto>();
        this.CreateMap<Car, ExportCarWithPartsDto>()
            .ForMember(d => d.Parts, opt => opt.MapFrom(s => s.PartsCars.Select(pc => pc.Part).OrderByDescending(p => p.Price).ToArray()));

        //Customer
        this.CreateMap<ImportCustomerDto, Customer>();

        //Sale
        this.CreateMap<ImportSaleDto, Sale>();
        this.CreateMap<Customer, ExportSalesByCustomerDto>()
            .ForMember(d => d.BoughtCars, opt => opt.MapFrom(c => c.Sales.Count))
            .ForMember(d => d.SpentMoney, opt => opt.MapFrom(c => c.Sales.SelectMany(s => s.Car.PartsCars).Sum(pc => pc.Part.Price)));
        this.CreateMap<Sale, ExportSaleWithAppliedDiscountDto>();
    }
}