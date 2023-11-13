namespace ProductShop;

using Newtonsoft.Json;

using Data;
using DTOs.Import;
using ProductShop.Models;
using AutoMapper;

public class StartUp
{
    public static void Main()
    {
       ProductShopContext context = new ProductShopContext();

        string inputJson = File.ReadAllText(@"../../../Datasets/categories.json");

        string result = ImportCategories(context, inputJson);

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

    private static IMapper CreateMapper()
    {
        return new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductShopProfile>();
        }));
    }
}