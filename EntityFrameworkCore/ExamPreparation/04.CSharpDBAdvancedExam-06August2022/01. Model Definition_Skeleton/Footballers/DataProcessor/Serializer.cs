namespace Footballers.DataProcessor
{
    using Data;
    using Footballers.DataProcessor.ExportDto;
    using Newtonsoft.Json;
    using System.Globalization;
    using Trucks.Utilities;

    public class Serializer
    {
        public static string ExportCoachesWithTheirFootballers(FootballersContext context)
        {
            XmlHelper xmlHelper = new XmlHelper();

            ExportCoachDto[] coachDtos = context.Coaches
                .Where(c => c.Footballers.Any())
                .ToArray()
                .Select(c => new ExportCoachDto
                {
                    FootballersCount = c.Footballers.Count(),
                    CoachName = c.Name,
                    Footballers = c.Footballers
                    .Select(f => new ExportFootballerDto
                    {
                        Name = f.Name,
                        Position = f.PositionType.ToString()
                    })
                    .OrderBy(f => f.Name)
                    .ToArray()
                })
                .OrderByDescending(c => c.FootballersCount)
                .ThenBy(c => c.CoachName)
                .ToArray();

            return xmlHelper.Serialize(coachDtos, "Coaches");
        }

        public static string ExportTeamsWithMostFootballers(FootballersContext context, DateTime date)
        {
            var teams = context
                .Teams
                .Where(t => t.TeamsFootballers.Any(tf => tf.Footballer.ContractStartDate >= date))
                .ToArray()
                .Select(t => new
                {
                    t.Name,
                    Footballers = t.TeamsFootballers
                        .Where(f => f.Footballer.ContractStartDate >= date)
                        .OrderByDescending(tf => tf.Footballer.ContractEndDate)
                        .ThenBy(tf => tf.Footballer.Name)
                        .Select(tf => new
                        {
                            FootballerName = tf.Footballer.Name,
                            ContractStartDate = tf.Footballer.ContractStartDate.ToString("d", CultureInfo.InvariantCulture),
                            ContractEndDate = tf.Footballer.ContractEndDate.ToString("d", CultureInfo.InvariantCulture),
                            BestSkillType = tf.Footballer.BestSkillType.ToString(),
                            PositionType = tf.Footballer.PositionType.ToString()
                        })
                        .ToArray()
                })
                .Where(t => t.Footballers.Any(f => DateTime.Parse(f.ContractStartDate, CultureInfo.InvariantCulture) >= date))
                .OrderByDescending(t => t.Footballers.Length)
                .ThenBy(t => t.Name)
                .Take(5)
                .ToArray();

            return JsonConvert.SerializeObject(teams, Formatting.Indented);
        }
    }
}