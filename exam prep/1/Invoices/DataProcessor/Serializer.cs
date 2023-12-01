namespace Invoices.DataProcessor
{
    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.Data.Models.Enums;
    using Invoices.DataProcessor.ExportDto;
    using Newtonsoft.Json;
    using System.Globalization;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportClientsWithTheirInvoices(InvoicesContext context, DateTime date)
        {
            var clients=context.Clients.Where(x=>x.Invoices.Any(y=>y.IssueDate>date))
                .Select(x=>new ClientsExportDTO()
                {
                    InvoicesCount = x.Invoices.Count,
                    ClientName=x.Name,
                    VatNumber=x.NumberVat,
                    Invoices=x.Invoices
                    .OrderBy(x=>x.IssueDate)
                    .ThenByDescending(x=>x.DueDate)
                    .Select(y=>new InvoiceExportDto()
                    {
                        InvoiceNumber=y.Number,
                        Amount=decimal.Parse(String.Format("{0:0.##}", y.Amount)),
                        DueDate=y.DueDate.ToString("d", CultureInfo.InvariantCulture),
                        CurrencyType=y.CurrencyType.ToString()
                    }).ToArray(),
                })
                .OrderByDescending(x=>x.InvoicesCount)
                .ThenBy(x=>x.ClientName).ToArray();

            return XMLSerializer(clients, "Clients");
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

        public static string ExportProductsWithMostClients(InvoicesContext context, int nameLength)
        {
            var products=context.Products
                .Where(x => x.ProductsClients.Any(y => y.Client.Name.Length >= nameLength))
                .Select(x => new
                {
                    Name = x.Name,
                    Price = decimal.Parse(String.Format("{0:0.##}",x.Price)),
                    Category = x.CategoryType.ToString(),
                    Clients = x.ProductsClients
                        .Where(y => y.Client.Name.Length >= nameLength)
                        .Select(y => new
                        {
                            Name = y.Client.Name,
                            NumberVat = y.Client.NumberVat
                        }).OrderBy(x => x.Name).ToList()
                }).OrderByDescending(x => x.Clients.Count)
                .ThenBy(x => x.Name)
                .Take(5)
                .ToList();


            return JsonConvert.SerializeObject(products, Formatting.Indented);
        }
    }
}