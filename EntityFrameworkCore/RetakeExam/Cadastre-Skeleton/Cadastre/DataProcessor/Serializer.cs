using Cadastre.Data;
using Cadastre.Data.Enumerations;
using Cadastre.DataProcessor.ExportDtos;
using Newtonsoft.Json;
using System.Globalization;
using Trucks.Utilities;

namespace Cadastre.DataProcessor
{
    public class Serializer
    {
        public static string ExportPropertiesWithOwners(CadastreContext dbContext)
        {

            var date = DateTime.ParseExact("01/01/2000", "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var properties = dbContext.Properties
                 .Where(p => p.DateOfAcquisition >= date)
                 .ToArray()
                 .OrderByDescending(p => p.DateOfAcquisition)
                 .ThenBy(p => p.PropertyIdentifier)
                 .Select(p => new
                 {
                     PropertyIdentifier = p.PropertyIdentifier,
                     Area = p.Area,
                     Address = p.Address,
                     DateOfAcquisition = p.DateOfAcquisition.ToString("dd/MM/yyyy"),
                     Owners = p.PropertiesCitizens
                     .Select(pc => new
                     {
                         LastName = pc.Citizen.LastName,
                         MaritalStatus = pc.Citizen.MaritalStatus.ToString(),
                     })
                     .OrderBy(pc => pc.LastName)
                     .ToArray()
                 })
                 .ToArray();

            return JsonConvert.SerializeObject(properties, Formatting.Indented);
        }

        public static string ExportFilteredPropertiesWithDistrict(CadastreContext dbContext)
        {
            XmlHelper xmlHelper = new XmlHelper();
            ExportPropertyDto[] properties = dbContext.Properties
                .Where(p => p.Area >= 100)
                .ToArray()
                .OrderByDescending(p => p.Area)
                .ThenBy(p => p.DateOfAcquisition)
                .Select(p => new ExportPropertyDto
                {
                    PropertyIdentifier = p.PropertyIdentifier,
                    Area = p.Area,
                    DateOfAcquisition = p.DateOfAcquisition.ToString("dd/MM/yyyy"),
                    PostalCode = p.District.PostalCode
                })
                .ToArray();

            return xmlHelper.Serialize(properties, "Properties");
        }
    }
}
