namespace Trucks.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using Trucks.Data.Models;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedDespatcher
            = "Successfully imported despatcher - {0} with {1} trucks.";

        private const string SuccessfullyImportedClient
            = "Successfully imported client - {0} with {1} trucks.";

        public static string ImportDespatcher(TrucksContext context, string xmlString)
        {
            StringBuilder sb=new StringBuilder();
            var despatchers=XMLDeserializer<DespatcherImportDto[]>(xmlString, "Despatchers");

            List<Despatcher>validDespatchers=new List<Despatcher>();

            foreach (var despatcher in despatchers)
            {
                if (!IsValid(despatcher))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var tempDespatcher=new Despatcher()
                {
                    Name = despatcher.Name,
                    Position = despatcher.Position,
                };

                foreach (var truck in despatcher.Trucks)
                {
                    if (!IsValid(truck))
                    {
                        sb.AppendLine(ErrorMessage);
                    }
                    else
                    {
                        tempDespatcher.Trucks.Add(new Truck()
                        {
                            RegistrationNumber = truck.RegistrationNumber,
                            VinNumber = truck.VinNumber,
                            TankCapacity = truck.TankCapacity,
                            CargoCapacity = truck.CargoCapacity,
                            CategoryType = (CategoryType)truck.CategoryType,
                            MakeType = (MakeType)truck.MakeType,
                            Despatcher=tempDespatcher
                        });
                    }
                 
                    validDespatchers.Add(tempDespatcher);
                }
                sb.AppendLine(string.Format(SuccessfullyImportedDespatcher, tempDespatcher.Name, tempDespatcher.Trucks.Count));
            }

            context.AddRange(validDespatchers);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }
        public static string ImportClient(TrucksContext context, string jsonString)
        {
            var validTruckIds=context.Trucks.Select(x => x.Id).ToList();

            var clientsdto = JsonConvert.DeserializeObject<ClientImportDto[]>(jsonString);

            var validClients=new List<Client>();

            StringBuilder sb = new StringBuilder();

            foreach (var client in clientsdto)
            {
                if (!IsValid(client)||client.Type== "usual")
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var tempClinet = new Client()
                {
                    Name = client.Name,
                    Nationality = client.Nationality,
                    Type = client.Type,
                };

                foreach (var trucks in client.Trucks.Distinct())
                {
                    if (!validTruckIds.Contains(trucks))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    tempClinet.ClientsTrucks.Add(new ClientTruck()
                    {
                        TruckId=trucks,
                        Client=tempClinet
                    });
                }

                sb.AppendLine(string.Format(SuccessfullyImportedClient, tempClinet.Name, tempClinet.ClientsTrucks.Count));
                validClients.Add(tempClinet);
            }
            context.AddRange(validClients);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }

        private static T XMLDeserializer<T>(string xmlString, string root)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(root));

            using TextReader reader = new StringReader(xmlString);

            return (T)serializer.Deserialize(reader);
        }

    }
}