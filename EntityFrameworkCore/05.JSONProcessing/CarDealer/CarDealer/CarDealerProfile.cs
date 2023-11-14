namespace CarDealer;

using AutoMapper;

using DTOs.Import;
using Models;

public class CarDealerProfile : Profile
{
    public CarDealerProfile()
    {
        this.CreateMap<ImportSupplierDto, Supplier>();
        this.CreateMap<ImportPartDto, Part>();

    }
}
