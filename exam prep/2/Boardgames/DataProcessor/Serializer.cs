namespace Boardgames.DataProcessor
{
    using Boardgames.Data;
    using Boardgames.DataProcessor.ExportDto;
    using Newtonsoft.Json;
    using System.Xml.Serialization;

    public class Serializer
    {

        public static string ExportSellersWithMostBoardgames(BoardgamesContext context, int year, double rating)
        {

            var sellers=context.Sellers
                .Where(x => x.BoardgamesSellers.Any(y => y.Boardgame.YearPublished >= year && y.Boardgame.Rating <= rating))
                .Select(x => new
                {
                    Name = x.Name,
                    Website = x.Website,
                    Boardgames = x.BoardgamesSellers
                    .Where(y => y.Boardgame.YearPublished >= year && y.Boardgame.Rating <= rating)
                    .Select(y => new
                    {
                        Name = y.Boardgame.Name,
                        Rating = y.Boardgame.Rating,
                        Mechanics = y.Boardgame.Mechanics,
                        Category = y.Boardgame.CategoryType.ToString(),
                    }).OrderByDescending(y => y.Rating)
                    .ThenBy(y => y.Name).ToList()
                }).OrderByDescending(x => x.Boardgames.Count)
                .ThenBy(x => x.Name)
                .Take(5)
                .ToList();

            return JsonConvert.SerializeObject(sellers, Formatting.Indented);
        }

        public static string ExportCreatorsWithTheirBoardgames(BoardgamesContext context)
        {
            var creators=context.Creators.Where(x => x.Boardgames.Any())
                .Select(x => new CreatorExportDto()
                {
                    BoardgamesCount = x.Boardgames.Count,
                    CreatorName = x.FirstName + " " + x.LastName,
                    Boardgames = x.Boardgames.Select(y => new BoardGameExportDto()
                    {
                        BoardgameName = y.Name,
                        BoardgameYearPublished = y.YearPublished,
                    }).OrderBy(x => x.BoardgameName).ToArray(),
                }).OrderByDescending(x => x.BoardgamesCount)
                .ThenBy(x => x.CreatorName).ToArray();

            return XMLSerializer<CreatorExportDto[]>(creators, "Creators");
        }

       
        private static string XMLSerializer<T>(T obj, string root)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(root));

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using StringWriter writer = new StringWriter();

            serializer.Serialize(writer, obj, ns);

            return writer.ToString().TrimEnd();
        }
    }
}