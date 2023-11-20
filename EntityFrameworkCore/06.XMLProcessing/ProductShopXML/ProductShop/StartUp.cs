namespace ProductShop;

using AutoMapper;

using ProductShop.Data;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using ProductShop.Utilities;

public class StartUp
{
    public static void Main()
    {
        using ProductShopContext context = new ProductShopContext();
        string inputXml =
            File.ReadAllText("../../../Datasets/categories.xml");

        string result = ImportCategories(context, inputXml);

        Console.WriteLine(result);
    }

    public static string ImportUsers(ProductShopContext context, string inputXml)
    {
        IMapper mapper = CreateMapper();

        XmlHelper xmlHelper = new XmlHelper();

        ImportUserDto[] userDtos = xmlHelper.Deserialize<ImportUserDto[]>(inputXml, "Users");

        ICollection<User> validUsers = new HashSet<User>();

        foreach (ImportUserDto userDto in userDtos)
        {
            if (string.IsNullOrEmpty(userDto.FirstName) || string.IsNullOrEmpty(userDto.LastName))
            {
                continue;
            }

            User user = mapper.Map<User>(userDto);
            validUsers.Add(user);
        }

        context.Users.AddRange(validUsers);
        context.SaveChanges();

        return $"Successfully imported {validUsers.Count}";
    }

    public static string ImportProducts(ProductShopContext context, string inputXml)
    {
        IMapper mapper = CreateMapper();

        XmlHelper xmlHelper = new XmlHelper();

        ImportProductDto[] productDtos = xmlHelper.Deserialize<ImportProductDto[]>(inputXml, "Products");

        ICollection<Product> validProducts = new HashSet<Product>();

        foreach (ImportProductDto productDto in productDtos)
        {
            if (string.IsNullOrEmpty(productDto.Name))
            {
                continue;
            }

            Product product = mapper.Map<Product>(productDto);
            validProducts.Add(product);
        }
        context.Products.AddRange(validProducts);
        context.SaveChanges();

        return $"Successfully imported {validProducts.Count}";
    }

    public static string ImportCategories(ProductShopContext context, string inputXml)
    {
        IMapper mapper = CreateMapper();

        XmlHelper xmlHelper = new XmlHelper();

        ImportCategoryDto[] categoryDtos = xmlHelper.Deserialize<ImportCategoryDto[]>(inputXml, "Categories");

        ICollection<Category> validCategories = new HashSet<Category>();

        foreach (ImportCategoryDto categoryDto in categoryDtos)
        {
            if (string.IsNullOrEmpty(categoryDto.Name))
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
