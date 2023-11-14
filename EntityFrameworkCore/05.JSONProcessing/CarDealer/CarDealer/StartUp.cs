namespace CarDealer;

using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class StartUp
{
    public static void Main()
    {
        CarDealerContext context = new CarDealerContext();

        string inputJson = File.ReadAllText(@"../../../Datasets/parts.json");

        string result = ImportParts(context, inputJson);

        Console.WriteLine(result);
    }

    public static string ImportSuppliers(CarDealerContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        ImportSupplierDto[] supplierDtos = JsonConvert.DeserializeObject<ImportSupplierDto[]>(inputJson);

        ICollection<Supplier> validSuppliers = new HashSet<Supplier>();

        foreach (ImportSupplierDto supplierDto in supplierDtos)
        {
            Supplier supplier = mapper.Map<Supplier>(supplierDto);

            validSuppliers.Add(supplier);
        }

        context.Suppliers.AddRange(validSuppliers);
        context.SaveChanges();

        return $"Successfully imported {validSuppliers.Count}.";
    }

    public static string ImportParts(CarDealerContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        ImportPartDto[] partDtos = JsonConvert.DeserializeObject<ImportPartDto[]>(inputJson);

        Part[] validParts = mapper.Map<Part[]>(partDtos);

        int[] supplierIds = context.Suppliers
            .Select(x => x.Id)
            .ToArray();

        Part[] partsWhitvalidSuppliers = validParts
            .Where(p => supplierIds
                    .Contains(p.SupplierId))
            .ToArray();

        context.Parts.AddRange(partsWhitvalidSuppliers);
        context.SaveChanges();

        return $"Successfully imported {partsWhitvalidSuppliers.Count()}.";
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