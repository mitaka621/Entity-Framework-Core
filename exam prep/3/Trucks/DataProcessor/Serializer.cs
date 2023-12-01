namespace Trucks.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using System.Xml.Serialization;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ExportDto;

    public class Serializer
    {
       

        public static string ExportClientsWithMostTrucks(TrucksContext context, int capacity)
        {
            var clients=context.Clients
                .Where(x=>x.ClientsTrucks.Any(y=>y.Truck.TankCapacity>=capacity))
                .ToArray()
                .Select(x=>new
                {
                    x.Name,
                    Trucks=x.ClientsTrucks.Where(y => y.Truck.TankCapacity >= capacity).Select(y=>new
                    {
                        TruckRegistrationNumber=y.Truck.RegistrationNumber,
                        VinNumber=y.Truck.VinNumber,
                        TankCapacity=y.Truck.TankCapacity,
                        CargoCapacity=y.Truck.CargoCapacity,
                        CategoryType=y.Truck.CategoryType.ToString(),
                        MakeType=y.Truck.MakeType.ToString(),
                    }).OrderBy(x => Enum.Parse<MakeType>(x.MakeType))
                    .ThenByDescending(x => x.CargoCapacity).ToArray()

                }).OrderByDescending(x=>x.Trucks.Count())
                .ThenBy(x=>x.Name)
                .Take(10)
                .ToArray();

            return JsonConvert.SerializeObject(clients, Formatting.Indented);
        }

        public static string ExportDespatchersWithTheirTrucks(TrucksContext context)
        {
            var despatchers=context.Despatchers.Where(x=>x.Trucks.Any())
                .Select(x=>new DespatcherExportDto() 
                { 
                    TrucksCount=x.Trucks.Count(),
                    DespatcherName=x.Name,
                    Trucks=x.Trucks.Select(y=>new TrucksExportDto() 
                    {
                        RegistrationNumber=y.RegistrationNumber,
                        Make=y.MakeType.ToString(),
                    }).OrderBy(x=>x.RegistrationNumber).ToArray()
                }).OrderByDescending(x=>x.Trucks.Count())
                .ThenBy(x=>x.DespatcherName).ToArray();
            return XMLSerializer(despatchers, "Despatchers");
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
