namespace ProductShop;

using AutoMapper;
using AutoMapper.QueryableExtensions;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using ProductShop.Utilities;

public class StartUp
{
    public static void Main()
    {
        using ProductShopContext context = new ProductShopContext();
        //string inputXml =
            File.ReadAllText("../../../Datasets/categories-products.xml");

        string result = GetCategoriesByProductsCount(context);

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

    public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
    {
        IMapper mapper = CreateMapper();

        XmlHelper xmlHelper = new XmlHelper();

        ImportCategoryProductDto[] categoryProductDtos = xmlHelper.Deserialize<ImportCategoryProductDto[]>(inputXml, "CategoryProducts");

        ICollection<CategoryProduct> validCategoryProducts = new HashSet<CategoryProduct>();

        foreach (ImportCategoryProductDto categoryProductDto in categoryProductDtos)
        {
            if (string.IsNullOrEmpty(categoryProductDto.CategoryId.ToString()) ||
                string.IsNullOrEmpty(categoryProductDto.ProductId.ToString()))
            {
                continue;
            }

            CategoryProduct categoryProduct = mapper.Map<CategoryProduct>(categoryProductDto);

            validCategoryProducts.Add(categoryProduct);
        }

        context.CategoryProducts.AddRange(validCategoryProducts);
        context.SaveChanges();

        return $"Successfully imported {validCategoryProducts.Count}";
    }

    public static string GetProductsInRange(ProductShopContext context)
    {
        IMapper mapper = CreateMapper();

        XmlHelper xmlHelper = new XmlHelper();

        ExportProductInRange[] products = context.Products
            .Where(p => p.Price >= 500 && p.Price <= 1000)
            .OrderBy(p => p.Price)
            .Take(10)
            .ProjectTo<ExportProductInRange>(mapper.ConfigurationProvider)
            .ToArray();

        return xmlHelper.Serialize<ExportProductInRange[]>(products, "Products");
    }

    public static string GetSoldProducts(ProductShopContext context)
    {
        XmlHelper xmlHelper = new XmlHelper();

        ExportUserWithAtLeastOneSoldItem[] users = context.Users
            .Where(u => u.ProductsSold.Any())
            .Select(u => new ExportUserWithAtLeastOneSoldItem
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Products = u.ProductsSold.Select(sp => new ExportUserProductDto
                {
                    Name = sp.Name,
                    Price = sp.Price
                })
                .ToArray()
            })
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Take(5)
            .ToArray();

        return xmlHelper.Serialize<ExportUserWithAtLeastOneSoldItem[]>(users, "Users");
    }

    public static string GetCategoriesByProductsCount(ProductShopContext context)
    {
        IMapper mapper = CreateMapper();
        XmlHelper xmlHelper = new XmlHelper();

        ExportCategoriesDto[] categoriesDtos = context.Categories
            .Select(c => new ExportCategoriesDto
            {
                Name = c.Name,
                Count = c.CategoryProducts.Count(),
                AveragePrice = c.CategoryProducts.Average(cp => cp.Product.Price),
                TotalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price)

            })
            .OrderByDescending(c => c.Count)
            .ThenBy(c => c.TotalRevenue)
            .ToArray();

        return xmlHelper.Serialize<ExportCategoriesDto[]>(categoriesDtos, "Categories");
    }

    private static IMapper CreateMapper()
    {
        return new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductShopProfile>();
        }));
    }
}
