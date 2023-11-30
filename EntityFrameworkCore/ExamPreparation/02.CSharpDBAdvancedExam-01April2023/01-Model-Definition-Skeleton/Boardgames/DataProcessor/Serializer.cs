namespace Boardgames.DataProcessor;

using Boardgames.Data;
using Boardgames.DataProcessor.ExportDto;
using Boardgames.Utilities;
using Newtonsoft.Json;

public class Serializer
{
    public static string ExportCreatorsWithTheirBoardgames(BoardgamesContext context)
    {
        ExportCreatorDto[] creators = context.Creators
            .Where(c => c.Boardgames.Any())
            .Select(c => new ExportCreatorDto
            {
                CreatorName = c.FirstName + " " + c.LastName,
                BoardgamesCount = c.Boardgames.Count(),
                Boardgames = c.Boardgames
                .Select(b => new ExportBoardgameDto
                {
                    BoardgameName = b.Name,
                    BoardgameYearPublished = b.YearPublished,
                })
                .OrderBy(b => b.BoardgameName)
                .ToArray()
            })
            .OrderByDescending(b => b.Boardgames.Length)
            .ThenBy(b => b.CreatorName)
            .ToArray();

        XmlHelper xmlHelper = new XmlHelper();

        return xmlHelper.Serialize(creators, "Creators");
    }

    public static string ExportSellersWithMostBoardgames(BoardgamesContext context, int year, double rating)
    {
        var sellers = context.Sellers
            .Where(s => s.BoardgamesSellers.Any(bs => bs.Boardgame.YearPublished >= year 
            && bs.Boardgame.Rating <= rating))
            .Select(s => new
            {
                Name = s.Name,
                Website = s.Website,
                Boardgames = s.BoardgamesSellers
                .Where(bs => bs.Boardgame.YearPublished >= year
            && bs.Boardgame.Rating <= rating)
                .Select(bs => new
                {
                    Name = bs.Boardgame.Name,
                    Rating = bs.Boardgame.Rating,
                    Mechanics = bs.Boardgame.Mechanics,
                    Category = bs.Boardgame.CategoryType.ToString(),
                })
                .OrderByDescending(bs => bs.Rating)
                .ThenBy(bs => bs.Name)
                .ToArray()
            })
            .OrderByDescending(s => s.Boardgames.Length)
            .ThenBy(s => s.Name)
            .Take(5)
            .ToArray();

        return JsonConvert.SerializeObject(sellers, Formatting.Indented);
    }
}