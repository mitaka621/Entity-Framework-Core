namespace Boardgames.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Xml.Serialization;
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.Data.Models.Enums;
    using Boardgames.DataProcessor.ImportDto;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";

        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            StringBuilder sb=new StringBuilder();

            var Creatordbo = XMLDeserializer<CreatorImportDto[]>(xmlString, "Creators");
            
            List<Creator> validCreators=new List<Creator>();

            foreach (var item in Creatordbo)
            {
                if (!IsValid(item))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var tempCreatpr = new Creator()
                {
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                };
                foreach (var boardgame in item.Boardgames)
                {
                    if (!IsValid(boardgame))
                    {
                        sb.AppendLine(ErrorMessage);
                    }
                    else
                        tempCreatpr.Boardgames.Add(new Boardgame()
                        {
                            Name = boardgame.Name,
                            Rating = boardgame.Rating,
                            YearPublished = boardgame.YearPublished,
                            CategoryType=(CategoryType)boardgame.CategoryType,
                            Mechanics = boardgame.Mechanics,                          
                        });
                       
                }

                validCreators.Add(tempCreatpr);
                sb.AppendLine(String.Format(SuccessfullyImportedCreator, tempCreatpr.FirstName, tempCreatpr.LastName, tempCreatpr.Boardgames.Count));
            }


            context.AddRange(validCreators);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {
            var selllersdto = JsonConvert.DeserializeObject<List<SellerImportDto>>(jsonString);

            StringBuilder sb = new StringBuilder();
            var validboardgmaes = context.Boardgames.Select(x => x.Id).ToArray();

            List<Seller> validSellers=new List<Seller>();
            foreach (var seller in selllersdto)
            {
                if (!IsValid(seller))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var tempSeller=(new Seller()
                {
                    Name = seller.Name,
                    Address = seller.Address,
                    Country = seller.Country,
                    Website = seller.Website,
                });
                foreach (var boargame in seller.Boardgames.Distinct())
                {
                    if (validboardgmaes.Contains(boargame))
                    {
                        tempSeller.BoardgamesSellers.Add(new BoardgameSeller() { BoardgameId = boargame });
                    }
                    else
                        sb.AppendLine(ErrorMessage);
                }


                validSellers.Add(tempSeller);
                sb.AppendLine(String.Format(SuccessfullyImportedSeller, tempSeller.Name, tempSeller.BoardgamesSellers.Count()));
            }

            context.AddRange(validSellers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static T XMLDeserializer<T>(string xmlString, string root)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(root));

            using TextReader reader = new StringReader(xmlString);

            return (T)serializer.Deserialize(reader);
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
