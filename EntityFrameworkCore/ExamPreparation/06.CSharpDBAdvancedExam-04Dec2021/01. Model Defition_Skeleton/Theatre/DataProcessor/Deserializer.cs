namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
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
            StringBuilder stringBuilder = new StringBuilder();

            xmlHelper = new XmlHelper();

            ImportCastDto[] castDtos =
                xmlHelper.Deserialize<ImportCastDto[]>(xmlString, "Casts");

            ICollection<Cast> validCasts = new HashSet<Cast>();

            foreach (ImportCastDto castDto in castDtos)
            {

                if (!IsValid(castDto))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Cast cast = new Cast()
                {
                    FullName = castDto.FullName,
                    IsMainCharacter = bool.Parse(castDto.IsMainCharacter),
                    PhoneNumber = castDto.PhoneNumber,
                    PlayId = castDto.PlayId,
                };

                validCasts.Add(cast);
                stringBuilder.AppendLine(string.Format(SuccessfulImportActor, cast.FullName, cast.IsMainCharacter == true? "main" : "lesser"));
            }
            context.Casts.AddRange(validCasts);
            context.SaveChanges();

            return stringBuilder.ToString().TrimEnd();
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            StringBuilder stringBuilder = new StringBuilder();

            ImportTheatreDto[] theatreDtos = JsonConvert.DeserializeObject<ImportTheatreDto[]>(jsonString);

            ICollection<Theatre> validTheatres = new HashSet<Theatre>();

            foreach (ImportTheatreDto theatreDto in theatreDtos)
            {
                if (!IsValid(theatreDto))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Theatre theatre = new Theatre()
                {
                    Name = theatreDto.Name,
                    NumberOfHalls = theatreDto.NumberOfHalls,
                    Director = theatreDto.Director
                };

                foreach (var ticketDto in theatreDto.Tickets)
                {
                    if (!IsValid(ticketDto))
                    {
                        stringBuilder.AppendLine(ErrorMessage);
                        continue;
                    }
                    theatre.Tickets.Add(new Ticket
                    {
                        Price = ticketDto.Price,
                        RowNumber = ticketDto.RowNumber,
                        PlayId = ticketDto.PlayId
                    });
                }

                validTheatres.Add(theatre);
                stringBuilder.AppendLine(string.Format(SuccessfulImportTheatre, theatre.Name, theatre.Tickets.Count));

            }

            context.Theatres.AddRange(validTheatres);
            context.SaveChanges();


            return stringBuilder.ToString().TrimEnd();
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
