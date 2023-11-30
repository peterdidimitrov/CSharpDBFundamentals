namespace Invoices.DataProcessor
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using Boardgames.Utilities;
    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.Data.Models.Enums;
    using Invoices.DataProcessor.ImportDto;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedClients
            = "Successfully imported client {0}.";

        private const string SuccessfullyImportedInvoices
            = "Successfully imported invoice with number {0}.";

        private const string SuccessfullyImportedProducts
            = "Successfully imported product - {0} with {1} clients.";

        private static XmlHelper xmlHelper;

        public static string ImportClients(InvoicesContext context, string xmlString)
        {
            StringBuilder stringBuilder = new StringBuilder();

            xmlHelper = new XmlHelper();

            ImportClientDto[] clientDtos = xmlHelper.Deserialize<ImportClientDto[]>(xmlString, "Clients");

            ICollection<Client> validClients = new HashSet<Client>();

            foreach (ImportClientDto clientDto in clientDtos)
            {
                if (!IsValid(clientDto))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                ICollection<Address> validAddresses = new HashSet<Address>();

                foreach (ImportAddressDto addressDto in clientDto.Addresses)
                {
                    if (!IsValid(addressDto))
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }

                    Address address = new Address()
                    {
                        StreetName = addressDto.StreetName,
                        StreetNumber = addressDto.StreetNumber,
                        PostCode = addressDto.PostCode,
                        City = addressDto.City,
                        Country = addressDto.Country
                    };

                    validAddresses.Add(address);
                }

                Client client = new Client()
                {
                    Name = clientDto.Name,
                    NumberVat = clientDto.NumberVat,
                    Addresses = validAddresses
                };

                validClients.Add(client);
                stringBuilder.AppendLine(string.Format(SuccessfullyImportedClients, client.Name));
            }
            context.Clients.AddRange(validClients);
            context.SaveChanges();

            return stringBuilder.ToString().TrimEnd();
        }

        public static string ImportInvoices(InvoicesContext context, string jsonString)
        {
            StringBuilder stringBuilder = new StringBuilder();

            ImportInvoiceDto[] invoiceDtos = JsonConvert.DeserializeObject<ImportInvoiceDto[]>(jsonString);

            ICollection<Invoice> validInvoices = new HashSet<Invoice>();

            ICollection<int> existingClientIds = context.Clients
            .Select(c => c.Id)
            .ToArray();

            foreach (ImportInvoiceDto invoiceDto in invoiceDtos)
            {
                if (!IsValid(invoiceDto))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                if (invoiceDto.DueDate < invoiceDto.IssueDate)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                if (!existingClientIds.Contains(invoiceDto.ClientId))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                if (invoiceDto.DueDate == DateTime.ParseExact("01/01/0001", "dd/MM/yyyy", CultureInfo.InvariantCulture) || invoiceDto.IssueDate == DateTime.ParseExact("01/01/0001", "dd/MM/yyyy", CultureInfo.InvariantCulture))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Invoice invoice = new Invoice()
                {
                    Number = invoiceDto.Number,
                    IssueDate = invoiceDto.DueDate,
                    DueDate = invoiceDto.DueDate,
                    Amount = invoiceDto.Amount,
                    CurrencyType = invoiceDto.CurrencyType,
                    ClientId = invoiceDto.ClientId,
                };

                validInvoices.Add(invoice);
                stringBuilder.AppendLine(string.Format(SuccessfullyImportedInvoices, invoiceDto.Number));
            }
            context.Invoices.AddRange(validInvoices);
            context.SaveChanges();

            return stringBuilder.ToString().TrimEnd();
        }

        public static string ImportProducts(InvoicesContext context, string jsonString)
        {
            StringBuilder stringBuilder = new StringBuilder();

            ImportProductDto[] productDtos = JsonConvert.DeserializeObject<ImportProductDto[]>(jsonString);

            ICollection<Product> validProducts = new HashSet<Product>();

            ICollection<int> existingClientIds = context.Clients
            .Select(c => c.Id)
            .ToArray();

            foreach (ImportProductDto productDto in productDtos)
            {
                if (!IsValid(productDto))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                if (!Enum.IsDefined(typeof(CategoryType), productDto.CategoryType))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Product product = new Product()
                {
                    Name = productDto.Name,
                    Price = productDto.Price,
                    CategoryType = productDto.CategoryType
                };

                foreach (int clientId in productDto.ClientIds.Distinct())
                {
                    if (!existingClientIds.Contains(clientId))
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }

                    ProductClient productClient = new ProductClient()
                    {
                        Product = product,
                        ClientId = clientId
                    };
                    product.ProductsClients.Add(productClient);
                }

                validProducts.Add(product);
                stringBuilder.AppendLine(string.Format(SuccessfullyImportedProducts, product.Name, product.ProductsClients.Count));
            }

            context.Products.AddRange(validProducts);
            context.SaveChanges();

            return stringBuilder.ToString().TrimEnd();
        }

        public static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}