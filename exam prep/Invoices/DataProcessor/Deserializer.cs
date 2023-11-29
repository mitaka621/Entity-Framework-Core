namespace Invoices.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using AutoMapper;
    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.Data.Models.Enums;
    using Invoices.DataProcessor.ImportDto;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedClients
            = "Successfully imported client {0}.";

        private const string SuccessfullyImportedInvoices
            = "Successfully imported invoice with number {0}.";

        private const string SuccessfullyImportedProducts
            = "Successfully imported product - {0} with {1} clients.";


        public static string ImportClients(InvoicesContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            List<ImportClientDto> clientsDto =
                XMLDeserializer<List<ImportClientDto>>(xmlString, "Clients");

            List<Client> clients = new List<Client>();

            foreach (var client in clientsDto)
            {
                if (!IsValid(client))
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                var clientToAdd = new Client
                {
                    Name = client.Name,
                    NumberVat = client.NumberVat
                };

                foreach (var address in client.Addresses)
                {
                    if (IsValid(address))
                    {
                        clientToAdd.Addresses.Add(new Address()
                        {
                            City = address.City,
                            Country = address.Country,
                            PostCode = address.PostCode,
                            StreetName = address.StreetName,
                            StreetNumber = address.StreetNumber
                        });
                    }
                    else
                    {
                        sb.AppendLine(ErrorMessage);
                    }
                }

                clients.Add(clientToAdd);
                sb.AppendLine(string.Format(SuccessfullyImportedClients, client.Name));
            }

            context.Clients.AddRange(clients);
            context.SaveChanges();

            return sb.ToString().TrimEnd();

        }

        public static string ImportInvoices(InvoicesContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();


            var invoicesDto = JsonConvert.DeserializeObject<Invoice[]>(jsonString); ;

            List<Invoice> invoices = new List<Invoice>();



            foreach (var invoice in invoicesDto)
            {
                if (!IsValid(invoice) || invoice.DueDate < invoice.IssueDate)
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }
                if (invoice.DueDate == DateTime.ParseExact("01/01/0001", "dd/MM/yyyy", CultureInfo.InvariantCulture) || invoice.IssueDate == DateTime.ParseExact("01/01/0001", "dd/MM/yyyy", CultureInfo.InvariantCulture))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                var invoiceToAdd = new Invoice
                {
                    Number = invoice.Number,
                    IssueDate = invoice.IssueDate,
                    DueDate = invoice.DueDate,
                    ClientId = invoice.ClientId,
                    Amount = invoice.Amount,
                    CurrencyType = invoice.CurrencyType
                };

                invoices.Add(invoiceToAdd);
                sb.AppendLine(string.Format(SuccessfullyImportedInvoices, invoice.Number));
            }

            context.Invoices.AddRange(invoices);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProducts(InvoicesContext context, string jsonString)
        {
            var validUsers=context.Clients.Select(x=>x.Id).ToList();
            StringBuilder sb=new StringBuilder();
            var products = JsonConvert.DeserializeObject<ProductDTO[]>(jsonString);

            List<Product> validProducts = new List<Product>();
            foreach (var product in products)
            {
                if (!IsValid(product))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Product tempProd=new Product() {
                    Name = product.Name,
                    Price = product.Price,
                    CategoryType=product.CategoryType,
                };
                foreach (var client in product.ClientsIDs.Distinct())
                {
                    if (validUsers.Contains(client))
                    {
                        tempProd.ProductsClients.Add(new ProductClient() { ClientId = client });
                    }
                    else
                        sb.AppendLine(ErrorMessage);
                }
                validProducts.Add(tempProd);
                sb.AppendLine(string.Format(SuccessfullyImportedProducts,tempProd.Name,tempProd.ProductsClients.Count));
            }

            context.AddRange(validProducts);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static T XMLDeserializer<T>(string xmlString, string root)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(root));

            using TextReader reader = new StringReader(xmlString);
         
            return (T)serializer.Deserialize(reader);
        }

        private static Mapper GetMapper()
        {
            var cfg = new MapperConfiguration(c => c.AddProfile<InvoicesProfile>());
            return new Mapper(cfg);
        }

        public static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
