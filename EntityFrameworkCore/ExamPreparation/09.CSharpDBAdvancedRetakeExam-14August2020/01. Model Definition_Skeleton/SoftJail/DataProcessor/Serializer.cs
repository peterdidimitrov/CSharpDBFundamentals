namespace SoftJail.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System.Runtime.Intrinsics.Arm;
    using Trucks.Utilities;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners = context.Prisoners
                .Where(p => ids.Contains(p.Id))
            .Select(p => new
                {
                    Id = p.Id,
                    Name = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    Officers = p.PrisonerOfficers
                    .Select(po => new
                    {
                        OfficerName = po.Officer.FullName,
                        Department = po.Officer.Department.Name
                    })
                    .OrderBy(o => o.OfficerName)
                    .ToArray(),
                TotalOfficerSalary = Math.Round(p.PrisonerOfficers.Sum(po => po.Officer.Salary), 2)
                })
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToArray();


            return JsonConvert.SerializeObject(prisoners, Formatting.Indented);
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            XmlHelper xmlHelper = new XmlHelper();

            string[] prisonersArray = prisonersNames.Split(',').ToArray();

            ExportPrisonerDto[] prisoners = context.Prisoners
                .Where(p => prisonersArray.Contains(p.FullName))
               .Select(p => new ExportPrisonerDto
               {
                   Id = p.Id,
                   Name = p.FullName,
                   IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd"),
                   EncryptedMessages = p.Mails.Select(m => new ExportMessageDto
                   {
                       Description = string.Join("", m.Description.ToCharArray().Reverse()),
                   })
                   .ToArray(),
               })
               .OrderBy(p => p.Name)
               .ThenBy(p => p.Id)
               .ToArray();

            return xmlHelper.Serialize(prisoners, "Prisoners");
        }
    }
}