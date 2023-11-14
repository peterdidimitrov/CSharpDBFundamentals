namespace ProductShop;

using Newtonsoft.Json;

using Data;
using DTOs.Import;
using ProductShop.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductShop.DTOs.Export;
using AutoMapper.QueryableExtensions;
using Newtonsoft.Json.Serialization;

public class StartUp
{
    public static void Main()
    {
        ProductShopContext context = new ProductShopContext();

        //string inputJson = File.ReadAllText(@"../../../Datasets/categories-products.json");

        string result = GetUsersWithProducts(context);

        Console.WriteLine(result);
    }
    public static string ImportUsers(ProductShopContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        ImportUserDto[] userDtos = JsonConvert.DeserializeObject<ImportUserDto[]>(inputJson);

        ICollection<User> validUsers = new HashSet<User>();

        foreach (ImportUserDto userDto in userDtos)
        {
            User user = mapper.Map<User>(userDto);

            validUsers.Add(user);
        }

        context.Users.AddRange(validUsers);
        context.SaveChanges();

        return $"Successfully imported {validUsers.Count}";
    }

    public static string ImportProducts(ProductShopContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        ImportProductDto[] productDtos = JsonConvert.DeserializeObject<ImportProductDto[]>(inputJson);

        ICollection<Product> validProducts = new HashSet<Product>();

        foreach (ImportProductDto productDto in productDtos)
        {
            Product product = mapper.Map<Product>(productDto);

            validProducts.Add(product);
        }

        context.Products.AddRange(validProducts);
        context.SaveChanges();

        return $"Successfully imported {validProducts.Count}";
    }

    public static string ImportCategories(ProductShopContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        ImportCategoryDto[] categoryDtos = JsonConvert.DeserializeObject<ImportCategoryDto[]>(inputJson);

        ICollection<Category> validCategories = new HashSet<Category>();

        foreach (ImportCategoryDto categoryDto in categoryDtos)
        {
            if (String.IsNullOrEmpty(categoryDto.Name))
            {
                continue;
            }
            Category category = mapper.Map<Category>(categoryDto);

            validCategories.Add(category);
        }

        context.Categories.AddRange(validCategories);
        context.SaveChanges();

        return $"Successfully imported {validCategories.Count}";
    }

    public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
    {
        IMapper mapper = CreateMapper();

        ImportCategoryProductDto[] categoryProductDtos = JsonConvert.DeserializeObject<ImportCategoryProductDto[]>(inputJson);

        ICollection<CategoryProduct> categoryProducts = new HashSet<CategoryProduct>();

        foreach (ImportCategoryProductDto categoryProductDto in categoryProductDtos)
        {
            CategoryProduct categoryProduct = mapper.Map<CategoryProduct>(categoryProductDto);

            categoryProducts.Add(categoryProduct);
        }

        context.CategoriesProducts.AddRange(categoryProducts);
        context.SaveChanges();

        return $"Successfully imported {categoryProducts.Count}";
    }

    public static string GetProductsInRange(ProductShopContext context)
    {
        //var products = context.Products
        //    .Where(p => p.Price >= 500 && p.Price <= 1000)
        //    .OrderBy(p => p.Price)
        //    .Select(p => new
        //    {
        //        name = p.Name,
        //        price = p.Price,
        //        seller = p.Seller.FirstName + " " + p.Seller.LastName
        //    })
        //    .AsNoTracking()
        //    .ToArray();

        //DTO + AutoMapper
        IMapper mapper = CreateMapper();

        ExportProductInRangeDto[] productDtos = context.Products
            .Where(p => p.Price >= 500 && p.Price <= 1000)
            .OrderBy(p => p.Price)
            .AsNoTracking()
            .ProjectTo<ExportProductInRangeDto>(mapper.ConfigurationProvider)
            .ToArray();

        return JsonConvert.SerializeObject(productDtos, Formatting.Indented);
    }

    public static string GetSoldProducts(ProductShopContext context)
    {
        IContractResolver contractResolver = ConfigureCamelCaseNaming();

        var usersWhitSoldProducts = context.Users
            .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Select(u => new
            {
                u.FirstName,
                u.LastName,
                SoldProducts = u.ProductsSold
                    .Where(p => p.Buyer != null)
                    .Select(p => new
                    {
                        p.Name,
                        p.Price,
                        BuyerFirstName = p.Buyer.FirstName,
                        BuyerLastName = p.Buyer.LastName
                    })
                    .ToArray()
            })
            .AsNoTracking()
            .ToArray();

        return JsonConvert.SerializeObject(usersWhitSoldProducts, Formatting.Indented, new JsonSerializerSettings
        {
            ContractResolver = contractResolver
        });
    }

    public static string GetCategoriesByProductsCount(ProductShopContext context)
    {
        IContractResolver contractResolver = ConfigureCamelCaseNaming();

        var categories = context.Categories
            .OrderByDescending(c => c.CategoriesProducts.Count())
            .Select(c => new
            {
                Category = c.Name,
                ProductsCount = c.CategoriesProducts.Count(),
                AveragePrice = (c.CategoriesProducts.Any() ? c.CategoriesProducts.Average(cp => cp.Product.Price) : 0).ToString("f2"),
                TotalRevenue = (c.CategoriesProducts.Any() ? c.CategoriesProducts.Sum(cp => cp.Product.Price) : 0).ToString("f2")
            })
            .AsNoTracking()
            .ToArray();

        return JsonConvert.SerializeObject(categories, Formatting.Indented, new JsonSerializerSettings
        {
            ContractResolver = contractResolver
        });
    }

    public static string GetUsersWithProducts(ProductShopContext context)
    {
        IContractResolver contractResolver = ConfigureCamelCaseNaming();

        var users = context.Users
            .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
            .Select(u => new
            {
                u.FirstName,
                u.LastName,
                u.Age,
                SoldProducts = new
                {
                    Count = u.ProductsSold.Count(p => p.Buyer != null),
                    Products = u.ProductsSold
                    .Where(p => p.Buyer != null)
                    .Select(p => new
                    {
                        p.Name,
                        p.Price
                    })
                    .ToArray()
                }
            })
            .OrderByDescending(u => u.SoldProducts.Count)
            .AsNoTracking()
            .ToArray();

        var userWrapperDto = new
        {
            UsersCount = users.Length,
            Users = users
        };


        return JsonConvert.SerializeObject(userWrapperDto, Formatting.Indented, new JsonSerializerSettings()
        {
            ContractResolver = contractResolver,
            NullValueHandling = NullValueHandling.Ignore
        });
    }

    private static IMapper CreateMapper()
    {
        return new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductShopProfile>();
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