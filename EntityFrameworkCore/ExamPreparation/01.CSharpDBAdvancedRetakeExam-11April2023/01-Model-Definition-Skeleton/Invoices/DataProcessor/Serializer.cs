namespace Invoices.DataProcessor;

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Boardgames.Utilities;
using Invoices.Data;
using Invoices.DataProcessor.ExportDto;
using Newtonsoft.Json;
using System.Text;

public class Serializer
{
    public static string ExportClientsWithTheirInvoices(InvoicesContext context, DateTime date)
    {

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<InvoicesProfile>();
        });

        StringBuilder sb = new StringBuilder();

        XmlHelper xmlHelper = new XmlHelper();

        ExportClientDto[] clientsDtos = context
            .Clients
            .Where(c => c.Invoices.Any(ci => ci.IssueDate >= date))
            .ProjectTo<ExportClientDto>(config)
            .OrderByDescending(c => c.InvoicesCount)
            .ThenBy(c => c.ClientName)
            .ToArray();

        return xmlHelper.Serialize(clientsDtos, "Clients");
    }

    public static string ExportProductsWithMostClients(InvoicesContext context, int nameLength)
    {

        var products = context.Products
            .Where(p => p.ProductsClients.Any(pc => pc.Client.Name.Length >= nameLength))
            .ToArray()
            .Select(p => new
            {
                Name = p.Name,
                Price = p.Price,
                Category = p.CategoryType.ToString(),
                Clients = p.ProductsClients
                .Where(pc => pc.Client.Name.Length >= nameLength)
                .Select(pc => new
                {
                    Name = pc.Client.Name,
                    NumberVat = pc.Client.NumberVat
                })
                .OrderBy(p => p.Name)
            })
            .OrderByDescending(p => p.Clients.Count())
            .ThenBy(p => p.Name)
            .Take(5)
            .ToArray();

        return JsonConvert.SerializeObject(products, Formatting.Indented);
    }
}