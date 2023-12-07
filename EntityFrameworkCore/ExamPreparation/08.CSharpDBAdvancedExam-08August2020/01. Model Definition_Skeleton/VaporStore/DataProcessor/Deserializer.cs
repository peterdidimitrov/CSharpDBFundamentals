namespace VaporStore.DataProcessor
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Net.Sockets;
    using System.Text;
    using Castle.Core.Internal;
    using Data;
    using Newtonsoft.Json;
    using Trucks.Utilities;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.ImportDto;

    public static class Deserializer
    {
        public const string ErrorMessage = "Invalid Data";

        public const string SuccessfullyImportedGame = "Added {0} ({1}) with {2} tags";

        public const string SuccessfullyImportedUser = "Imported {0} with {1} cards";

        public const string SuccessfullyImportedPurchase = "Imported {0} for {1}";

        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            StringBuilder stringBuilder = new StringBuilder();

            ImportGameDto[] gameDtos = JsonConvert.DeserializeObject<ImportGameDto[]>(jsonString);

            ICollection<Game> validGames = new HashSet<Game>();
            ICollection<Developer> validDevelopers = new HashSet<Developer>();
            ICollection<Genre> validGenres = new HashSet<Genre>();
            ICollection<Tag> validTags = new HashSet<Tag>();

            foreach (ImportGameDto gameDto in gameDtos)
            {
                if (!IsValid(gameDto) || gameDto.Name.ToLower() == "invalid")
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime releaseDate;
                bool isReleaseDateValid = DateTime.TryParseExact(gameDto.ReleaseDate, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out releaseDate);

                if (!isReleaseDateValid)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                if (gameDto.Tags.Length == 0)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Game game = new Game()
                {
                    Name = gameDto.Name,
                    Price = gameDto.Price,
                    ReleaseDate = releaseDate
                };

                Developer gameDev = validDevelopers
                    .FirstOrDefault(d => d.Name == gameDto.Developer);

                if (gameDev == null)
                {
                    Developer newGameDev = new Developer()
                    {
                        Name = gameDto.Developer,
                    };

                    validDevelopers.Add(newGameDev);

                    game.Developer = newGameDev;
                }
                else
                {
                    game.Developer = gameDev;
                }

                Genre gameGenre = validGenres
                    .FirstOrDefault(g => g.Name == gameDto.Genre);

                if (gameGenre == null)
                {
                    Genre newGenre = new Genre()
                    {
                        Name = gameDto.Genre,
                    };

                    validGenres.Add(newGenre);

                    game.Genre = newGenre;
                }
                else
                {
                    game.Genre = gameGenre;
                }

                foreach (string tagName in gameDto.Tags)
                {
                    if (String.IsNullOrEmpty(tagName))
                    {
                        continue;
                    }

                    Tag gameTag = validTags
                        .FirstOrDefault(t => t.Name == tagName);

                    if (gameTag == null)
                    {
                        Tag newGameTag = new Tag()
                        {
                            Name = tagName
                        };

                        validTags.Add(newGameTag);
                        game.GameTags.Add(new GameTag()
                        {
                            Game = game,
                            Tag = newGameTag
                        });
                    }
                    else
                    {
                        game.GameTags.Add(new GameTag()
                        {
                            Game = game,
                            Tag = gameTag
                        });
                    }
                }

                if (game.GameTags.Count == 0)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                validGames.Add(game);
                stringBuilder.AppendLine(string.Format(SuccessfullyImportedGame, game.Name, game.Genre.Name, game.GameTags.Count));

            }

            context.Games.AddRange(validGames);
            context.SaveChanges();

            return stringBuilder.ToString().TrimEnd();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            StringBuilder stringBuilder = new StringBuilder();

            ImportUserDto[] userDtos = JsonConvert.DeserializeObject<ImportUserDto[]>(jsonString);

            ICollection<User> validUsers = new HashSet<User>();

            foreach (ImportUserDto userDto in userDtos)
            {
                if (!IsValid(userDto))
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                List<Card> userCards = new List<Card>();

                bool areAllCardsValid = true;

                foreach (var cardDto in userDto.Cards)
                {
                    if (!IsValid(cardDto))
                    {
                        areAllCardsValid = false;
                        break;
                    }

                    Object cardTypeRes;
                    bool isCardTypeValid = Enum.TryParse(typeof(CardType), cardDto.Type, out cardTypeRes);

                    if (!isCardTypeValid)
                    {
                        areAllCardsValid = false;
                        break;
                    }

                    CardType cardType = (CardType)cardTypeRes;

                    userCards.Add(new Card()
                    {
                        Number = cardDto.Number,
                        Cvc = cardDto.CVC,
                        Type = cardType
                    });
                }

                if (!areAllCardsValid)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                if (userCards.Count == 0)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                User user = new User()
                {
                    Username = userDto.Username,
                    FullName = userDto.FullName,
                    Email = userDto.Email,
                    Age = userDto.Age,
                    Cards = userCards
                };

                validUsers.Add(user);
                stringBuilder.AppendLine(string.Format(SuccessfullyImportedUser, user.Username, user.Cards.Count));

            }

            context.Users.AddRange(validUsers);
            context.SaveChanges();


            return stringBuilder.ToString().TrimEnd();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            XmlHelper xmlHelper = new XmlHelper();
            StringBuilder stringBuilder = new StringBuilder();

            ImportPurchasesDto[] purchaseDtos = xmlHelper.Deserialize<ImportPurchasesDto[]>(xmlString, "Purchases");

            ICollection<Purchase> purchases = new HashSet<Purchase>();

            foreach (ImportPurchasesDto purchaseDto in purchaseDtos)
            {
                if (!IsValid(purchaseDto))
                {
                    stringBuilder.Append(ErrorMessage);
                    continue;
                }

                object purchaseTypeObj;
                bool isPurchaseTypeValid =
                    Enum.TryParse(typeof(PurchaseType), purchaseDto.Type, out purchaseTypeObj);

                if (!isPurchaseTypeValid)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                PurchaseType purchaseType = (PurchaseType)purchaseTypeObj;

                DateTime date;
                bool isDateValid = DateTime.TryParseExact(purchaseDto.Date, "dd/MM/yyyy HH:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

                if (!isDateValid)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Card card = context
                   .Cards
                   .FirstOrDefault(c => c.Number == purchaseDto.Card);

                if (card == null)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Game game = context
                    .Games
                    .FirstOrDefault(g => g.Name == purchaseDto.Title);

                if (game == null)
                {
                    stringBuilder.AppendLine(ErrorMessage);
                    continue;
                }

                Purchase purchase = new Purchase()
                {
                    Type = purchaseType,
                    ProductKey = purchaseDto.Key,
                    Date = date,
                    Game = game,
                    Card = card

                };

                purchases.Add(purchase);
                stringBuilder.AppendLine(String.Format(SuccessfullyImportedPurchase, purchase.Game.Name, purchase.Card.User.Username));
            }

            context.Purchases.AddRange(purchases);
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
}