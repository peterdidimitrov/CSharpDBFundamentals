namespace Theatre.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.Data.Models.Enums;
    using Theatre.DataProcessor.ImportDto;
    using Trucks.Utilities;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";

        private static XmlHelper xmlHelper;


        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            var validGenres = new string[] { "Drama", "Comedy", "Romance", "Musical" };
            var minimumTime = new TimeSpan(1, 0, 0);

            StringBuilder stringBuilder = new StringBuilder();

            xmlHelper = new XmlHelper();

            ImportPlayDto[] playDtos =
                xmlHelper.Deserialize<ImportPlayDto[]>(xmlString, "Plays");

            ICollection<Play> validPlays = new HashSet<Play>();

            foreach (ImportPlayDto playDto in playDtos)
            {
                var currentTime = TimeSpan.ParseExact(playDto.Duration, "c", CultureInfo.InvariantCulture);

                if (!IsValid(playDto) ||
                    !validGenres.Contains(playDto.Genre) ||
                    (currentTime < minimumTime))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Play play = new Play()
                {
                    Title = playDto.Title,
                    Duration = TimeSpan.ParseExact(playDto.Duration, "c", CultureInfo.InvariantCulture),
                    Rating = playDto.Rating,
                    Genre = (Genre)Enum.Parse(typeof(Genre), playDto.Genre),
                    Description = playDto.Description,
                    Screenwriter = playDto.Screenwriter,
                };

                validPlays.Add(play);
                stringBuilder.AppendLine(string.Format(SuccessfulImportPlay, play.Title, play.Genre, play.Rating));
            }
            context.Plays.AddRange(validPlays);
            context.SaveChanges();

            return stringBuilder.ToString().TrimEnd();
        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            throw new NotImplementedException();
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            throw new NotImplementedException();
        }


        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
