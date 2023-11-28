namespace Trucks.DataProcessor;

using System.ComponentModel.DataAnnotations;
using System.Text;
using Data;
using Newtonsoft.Json;
using Trucks.Data.Models;
using Trucks.Data.Models.Enums;
using Trucks.DataProcessor.ImportDto;
using Trucks.Utilities;

public class Deserializer
{
    private const string ErrorMessage = "Invalid data!";

    private const string SuccessfullyImportedDespatcher
        = "Successfully imported despatcher - {0} with {1} trucks.";

    private const string SuccessfullyImportedClient
        = "Successfully imported client - {0} with {1} trucks.";

    private static XmlHelper xmlHelper;

    public static string ImportDespatcher(TrucksContext context, string xmlString)
    {
        StringBuilder stringBuilder = new StringBuilder();

        xmlHelper = new XmlHelper();

        ImportDespatcherDto[] despatcherDtos =
            xmlHelper.Deserialize<ImportDespatcherDto[]>(xmlString, "Despatchers");

        ICollection<Despatcher> validDespatcher = new HashSet<Despatcher>();

        foreach (ImportDespatcherDto despatcherDto in despatcherDtos)
        {
            if (!IsValid(despatcherDto))
            {
                stringBuilder.AppendLine(ErrorMessage);
                continue;
            }

            ICollection<Truck> validTrucks = new HashSet<Truck>();

            foreach (ImportTruckDto truckDto in despatcherDto.Trucks)
            {
                if (!IsValid(truckDto))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Truck truck = new Truck()
                {
                    RegistrationNumber = truckDto.RegistrationNumber,
                    VinNumber = truckDto.VinNumber,
                    TankCapacity = truckDto.TankCapacity,
                    CargoCapacity = truckDto.CargoCapacity,
                    CategoryType = (CategoryType)truckDto.CategoryType,
                    MakeType = (MakeType)truckDto.MakeType,
                };

                validTrucks.Add(truck);
            }

            Despatcher despatcher = new Despatcher()
            {
                Name = despatcherDto.Name,
                Position = despatcherDto.Position,
                Trucks = validTrucks
            };

            validDespatcher.Add(despatcher);
            stringBuilder.AppendLine(string.Format(SuccessfullyImportedDespatcher, despatcher.Name, validTrucks.Count));
        }
        context.Despatchers.AddRange(validDespatcher);
        context.SaveChanges();

        return stringBuilder.ToString().TrimEnd();
    }
    public static string ImportClient(TrucksContext context, string jsonString)
    {
        StringBuilder stringBuilder = new StringBuilder();

        ImportClientDto[] clientDtos = JsonConvert.DeserializeObject<ImportClientDto[]>(jsonString);

        ICollection<Client> validClients = new HashSet<Client>();

        ICollection<int> existingTruckIds = context.Trucks
            .Select(t => t.Id)
            .ToArray();

        foreach (ImportClientDto clientDto in clientDtos)
        {
            if (!IsValid(clientDto))
            {
                stringBuilder.AppendLine(ErrorMessage);
                continue;
            }

            if (clientDto.Type == "usual")
            {
                stringBuilder.AppendLine(ErrorMessage);
                continue;
            }

            Client client = new Client()
            {
                Name = clientDto.Name,
                Nationality = clientDto.Nationality,
                Type = clientDto.Type,
            };

            foreach (int truckId in clientDto.TruckIds.Distinct())
            {
                if (!existingTruckIds.Contains(truckId))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                ClientTruck clientTruck = new ClientTruck()
                {
                    Client = client,
                    TruckId = truckId
                };
                client.ClientsTrucks.Add(clientTruck);
            }

            validClients.Add(client);
            stringBuilder.AppendLine(string.Format(SuccessfullyImportedClient, client.Name, client.ClientsTrucks.Count));
        }

        context.Clients.AddRange(validClients);
        context.SaveChanges();

        return stringBuilder.ToString().TrimEnd();
    }

    private static bool IsValid(object dto)
    {
        var validationContext = new ValidationContext(dto);
        var validationResult = new List<ValidationResult>();

        return Validator.TryValidateObject(dto, validationContext, validationResult, true);
    }
}