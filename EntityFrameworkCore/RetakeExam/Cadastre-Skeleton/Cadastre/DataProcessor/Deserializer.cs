namespace Cadastre.DataProcessor
{
    using Cadastre.Data;
    using Cadastre.Data.Enumerations;
    using Cadastre.Data.Models;
    using Cadastre.DataProcessor.ImportDtos;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Data.SqlTypes;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using Trucks.Utilities;

    public class Deserializer
    {
        private const string ErrorMessage =
            "Invalid Data!";
        private const string SuccessfullyImportedDistrict =
            "Successfully imported district - {0} with {1} properties.";
        private const string SuccessfullyImportedCitizen =
            "Succefully imported citizen - {0} {1} with {2} properties.";

        public static string ImportDistricts(CadastreContext dbContext, string xmlDocument)
        {

            StringBuilder stringBuilder = new StringBuilder();
            XmlHelper xmlHelper = new XmlHelper();
            ImportDistrictDto[] districtDtos =
                xmlHelper.Deserialize<ImportDistrictDto[]>(xmlDocument, "Districts");

            ICollection<District> validDistricts = new HashSet<District>();

            foreach (ImportDistrictDto districtDto in districtDtos)
            {
                var uniqueDistricts = validDistricts.FirstOrDefault(d => d.Name == districtDto.Name);

                if (!IsValid(districtDto) || uniqueDistricts != null)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                District district = new District()
                {
                    Name = districtDto.Name,
                    PostalCode = districtDto.PostalCode,
                    Region = (Region)Enum.Parse(typeof(Region), districtDto.Region)
                };

                ICollection<Property> validProperties = new HashSet<Property>();

                foreach (ImportPropertyDto propertyDto in districtDto.Properties)
                {
                    var uniquePropery = validProperties.FirstOrDefault(p => p.PropertyIdentifier == propertyDto.PropertyIdentifier);

                    var uniquePropertyAddressInColection = validProperties.FirstOrDefault(p => p.Address == propertyDto.Address);

                    var uniquePropertyAddress = validDistricts.FirstOrDefault(d => d.Properties.Any(p => p.Address == propertyDto.Address));
                    var universePropertyAddressInDb = dbContext.Properties.FirstOrDefault(p => p.Address == propertyDto.Address);

                    var uniquePropertyIdentifier = validDistricts.FirstOrDefault(d => d.Properties.Any(p => p.PropertyIdentifier == propertyDto.PropertyIdentifier));
                    var uniquePropertyIdentifierInDb = dbContext.Properties.FirstOrDefault(p => p.PropertyIdentifier == propertyDto.PropertyIdentifier);

                    if (!IsValid(propertyDto) 
                        || uniquePropery != null 
                        || uniquePropertyAddress != null
                        || universePropertyAddressInDb != null
                        || uniquePropertyIdentifier != null
                        || uniquePropertyIdentifierInDb != null
                        || uniquePropertyAddressInColection != null
                        )
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }

                    Property property = new Property()
                    {
                        PropertyIdentifier = propertyDto.PropertyIdentifier,
                        Area = propertyDto.Area,
                        Details  = propertyDto.Details,
                        Address = propertyDto.Address,
                        DateOfAcquisition = DateTime.ParseExact(propertyDto.DateOfAcquisition, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                    };

                    validProperties.Add(property);

                    district.Properties.Add(property);
                }

                validDistricts.Add(district);
                stringBuilder.AppendLine(string.Format(SuccessfullyImportedDistrict, district.Name, district.Properties.Count));
            }
            dbContext.Districts.AddRange(validDistricts);
            dbContext.SaveChanges();

            return stringBuilder.ToString().TrimEnd();
        }

        public static string ImportCitizens(CadastreContext dbContext, string jsonDocument)
        {
            StringBuilder stringBuilder = new StringBuilder();

            ImportCitizenDto[] citizenDtos = JsonConvert.DeserializeObject<ImportCitizenDto[]>(jsonDocument);

            ICollection<Citizen> validCitizens = new HashSet<Citizen>();


            foreach (ImportCitizenDto citizenDto in citizenDtos)
            {

                var validMaritalStatus = new string[] { "Unmarried", "Married", "Divorced", "Widowed" };

                if (!IsValid(citizenDto) || !validMaritalStatus.Contains(citizenDto.MaritalStatus))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Citizen citizen = new Citizen()
                {
                   FirstName = citizenDto.FirstName,
                   LastName = citizenDto.LastName,
                   BirthDate = DateTime.ParseExact(citizenDto.BirthDate, "dd-MM-yyyy", CultureInfo.InvariantCulture),
                   MaritalStatus = (MaritalStatus)Enum.Parse(typeof(MaritalStatus), citizenDto.MaritalStatus),
                };

                List<PropertyCitizen> propertyCitizens = new List<PropertyCitizen>();

                foreach (int propertiesIds in citizenDto.Properties)
                {
                    PropertyCitizen propertyCitizen = new PropertyCitizen()
                    {
                        Citizen = citizen,
                        PropertyId = propertiesIds
                    };

                    propertyCitizens.Add(propertyCitizen);
                    citizen.PropertiesCitizens.Add(propertyCitizen);
                }

                validCitizens.Add(citizen);
                stringBuilder.AppendLine(string.Format(SuccessfullyImportedCitizen, citizen.FirstName, citizen.LastName, citizen.PropertiesCitizens.Count));
            }

            dbContext.Citizens.AddRange(validCitizens);
            dbContext.SaveChanges();

            return stringBuilder.ToString().Trim();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
