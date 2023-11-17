using CarDealer.Data;

namespace CarDealer;

using AutoMapper;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using CarDealer.Utilities;
using Newtonsoft.Json.Serialization;
using System.IO;

public class StartUp
{
    public static void Main()
    {
        using CarDealerContext context = new CarDealerContext();
        string inputXml =
            File.ReadAllText("../../../Datasets/parts.xml");

        string result = ImportParts(context, inputXml);

        Console.WriteLine(result);
    }
    public static string ImportSuppliers(CarDealerContext context, string inputXml)
    {
        IMapper mapper = CreateMapper();

        XmlHelper xmlHelper = new XmlHelper();

        ImportSupplierDto[] supplierDtos = xmlHelper.Deserialize<ImportSupplierDto[]>(inputXml, "Suppliers");

        ICollection<Supplier> validSuppliers = new HashSet<Supplier>();

        foreach (ImportSupplierDto supplierDto in supplierDtos)
        {
            if (string.IsNullOrEmpty(supplierDto.Name))
            {
                continue;
            }

            //Manual mapping without AutoMapper
            //Supplier supplier = new Supplier()
            //{
            //    Name  = supplierDto.Name,
            //    IsImporter = supplierDto.IsImporter
            //};
            Supplier supplier = mapper.Map<Supplier>(supplierDto);

            validSuppliers.Add(supplier);
        }

        context.Suppliers.AddRange(validSuppliers);
        context.SaveChanges();

        return $"Successfully imported {validSuppliers.Count}";
    }

    public static string ImportParts(CarDealerContext context, string inputXml)
    {
        IMapper mapper = CreateMapper();

        XmlHelper xmlHelper = new XmlHelper();

        ImportPartDto[] partDtos = xmlHelper.Deserialize<ImportPartDto[]>(inputXml, "Parts");

        ICollection<Part> validParts = new HashSet<Part>();

        foreach (ImportPartDto partDto in partDtos)
        {
            if (string.IsNullOrEmpty(partDto.Name))
            {
                continue;
            }

            if (!partDto.SupplierId.HasValue || !context.Suppliers.Any(s => s.Id == partDto.SupplierId))
            {
                continue;
            }

            Part part = mapper.Map<Part>(partDto);

            validParts.Add(part);
        }

        context.Parts.AddRange(validParts);
        context.SaveChanges();
        return $"Successfully imported {validParts.Count}";
    }

    private static IMapper CreateMapper()
    {
        return new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CarDealerProfile>();
        }));
    }

    private static IContractResolver ConfigureCamelCaseNaming()
    {
        return new DefaultContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy(false, true)
        };
    }
}