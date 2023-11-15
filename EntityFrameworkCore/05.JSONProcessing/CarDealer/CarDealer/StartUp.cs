namespace CarDealer;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using AutoMapper;
using Data;
using DTOs.Import;
using Models;
using Castle.Core.Resource;
using Microsoft.EntityFrameworkCore;

public class StartUp
{
    public static void Main()
    {
        CarDealerContext context = new CarDealerContext();

        //string inputJson = File.ReadAllText(@"../../../Datasets/sales.json");

        string result = GetTotalSalesByCustomer(context);

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

    public static string ImportCars(CarDealerContext context, string inputJson)
    {
        var carsDto = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);

        var cars = new List<Car>();
        var carParts = new List<PartCar>();


        foreach (var carDto in carsDto)
        {
            var car = new Car
            {
                Make = carDto.Make,
                Model = carDto.Model,
                TraveledDistance = carDto.TraveledDistance
            };

            foreach (var part in carDto.PartsId.Distinct())
            {
                var carPart = new PartCar()
                {
                    PartId = part,
                    Car = car
                };

                carParts.Add(carPart);
            }
            cars.Add(car);
        }

        context.Cars.AddRange(cars);

        context.PartsCars.AddRange(carParts);

        context.SaveChanges();

        return $"Successfully imported {context.Cars.Count()}.";
    }

    public static string ImportCustomers(CarDealerContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        ImportCustomerDto[] customerDtos = JsonConvert.DeserializeObject<ImportCustomerDto[]>(inputJson);

        Customer[] validCustomers = mapper.Map<Customer[]>(customerDtos);

        context.Customers.AddRange(validCustomers);
        context.SaveChanges();

        return $"Successfully imported {validCustomers.Count()}.";
    }

    public static string ImportSales(CarDealerContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        ImportSaleDto[] saleDtos = JsonConvert.DeserializeObject<ImportSaleDto[]>(inputJson);

        Sale[] validSales = mapper.Map<Sale[]>(saleDtos);

        context.Sales.AddRange(validSales);
        context.SaveChanges();

        return $"Successfully imported {validSales.Count()}.";
    }

    public static string GetOrderedCustomers(CarDealerContext context)
    {
        var customers = context.Customers
            .OrderBy(c => c.BirthDate)
            .Select(c => new
            {
                c.Name,
                BirthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                c.IsYoungDriver
            })
            .AsNoTracking()
            .ToArray();

        return JsonConvert.SerializeObject(customers, Formatting.Indented);
    }

    public static string GetCarsFromMakeToyota(CarDealerContext context)
    {
        var cars = context.Cars
            .Where(c => c.Make == "Toyota")
            .OrderBy(c => c.Model)
            .ThenByDescending(c => c.TraveledDistance)
            .Select(c => new
            {
                c.Id,
                c.Make,
                c.Model,
                c.TraveledDistance
            })
            .AsNoTracking()
            .ToArray();

        return JsonConvert.SerializeObject(cars, Formatting.Indented);
    }

    public static string GetLocalSuppliers(CarDealerContext context)
    {
        var suppliers = context.Suppliers
            .Where(s => s.IsImporter == false)
            .Select(s => new
            {
                s.Id,
                s.Name,
                PartsCount = s.Parts.Count
            })
            .AsNoTracking()
            .ToArray();

        return JsonConvert.SerializeObject(suppliers, Formatting.Indented);
    }

    public static string GetCarsWithTheirListOfParts(CarDealerContext context)
    {
        var cars = context.Cars
            .Select(c => new
            {
                car = new
                {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance
                },
                parts = c.PartsCars
                    .Select(pc => new
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price.ToString("f2")
                    })
                    .ToArray()
            })
            .AsNoTracking()
            .ToArray();

        return JsonConvert.SerializeObject(cars, Formatting.Indented);
    }

    public static string GetTotalSalesByCustomer(CarDealerContext context)
    {
        IContractResolver contractResolver = ConfigureCamelCaseNaming();
        var customers = context.Customers
            .Where(c => c.Sales.Any())
            .Select(c => new
            {
                FullName = c.Name,
                BoughtCars = c.Sales.Count(),
                SpentMoney = c.Sales.Sum(s => s.Car.PartsCars.Sum(pc => pc.Part.Price))
            })
            .OrderByDescending(c => c.SpentMoney)
            .ThenByDescending(c => c.BoughtCars)
            .AsNoTracking()
            .ToArray();

        return JsonConvert.SerializeObject(customers, Formatting.Indented, new JsonSerializerSettings()
        {
            ContractResolver = contractResolver,
            NullValueHandling = NullValueHandling.Ignore
        });
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