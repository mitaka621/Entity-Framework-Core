using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();
            string file = File.ReadAllText("../../../Datasets/sales.xml");
            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlSerializer deserializer=new XmlSerializer(typeof(ImportSupplyer[]), new XmlRootAttribute("Suppliers"));

            using TextReader reader = new StringReader(inputXml);
            var supplierDTO= (ImportSupplyer[])deserializer.Deserialize(reader);
            

            Mapper mapper = GetMapper();

            var suppliers = mapper.Map<Supplier[]>(supplierDTO);
            context.AddRange(suppliers);
            context.SaveChanges();
            return $"Successfully imported {suppliers.Length}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlSerializer deserializer =
                new XmlSerializer(typeof(PartsDTO[]), new XmlRootAttribute("Parts"));
            using TextReader reader = new StringReader(inputXml);

            var ids = context.Suppliers.Select(x => x.Id).ToArray();
            var partsDto = ((PartsDTO[])deserializer.Deserialize(reader)!)
                .Where(x => ids.Contains(x.SupplierId));

            Mapper mapper = GetMapper();   
            var parts = mapper.Map<Part[]>(partsDto);

            context.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {partsDto.Count()}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var carPartsIds=context.Parts.AsNoTracking().Select(x=>x.Id).ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CarDto[]), new XmlRootAttribute("Cars"));

            using TextReader reader = new StringReader(inputXml);
            var carsDto=(CarDto[])xmlSerializer.Deserialize(reader);
            Mapper mapper = GetMapper();

            List<Car> cars = new List<Car>();
            foreach (var carDto in carsDto)
            {
                Car tempCar=mapper.Map<Car>(carDto);
                var carPartsIDs= carDto.CarParts.DistinctBy(x=>x.Id).Where(x=> carPartsIds.Contains(x.Id)).ToArray();

          

                foreach (var id in carPartsIDs)
                {
                    tempCar.PartsCars.Add(new PartCar { Car = tempCar, PartId = id.Id });
                }
                
                cars.Add(tempCar);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}"; ;
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CustomerDTO[]), new XmlRootAttribute("Customers"));

            using TextReader reader = new StringReader(inputXml);

            CustomerDTO[] customersDTO = (CustomerDTO[])serializer.Deserialize(reader)!;

            Mapper mapper = GetMapper();

            var customers=mapper.Map<Customer[]>(customersDTO);

            context.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}";

        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SalesDTO[]), new XmlRootAttribute("Sales"));

            using TextReader reader = new StringReader(inputXml);

            var carIds=context.Cars.Select(x=>x.Id).ToList();
            var salesDTO = ((SalesDTO[])serializer.Deserialize(reader)!)
                .Where(x=>carIds.Contains(x.CarId));

            Mapper mapper= GetMapper();

            var sales=mapper.Map<Sale[]>(salesDTO);

            context.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            Mapper mapper = GetMapper();
            var cars =context.Cars.Where(x => x.TraveledDistance > 2000000).OrderBy(x => x.Make).ThenBy(x => x.Model).Take(10)               
                .ProjectTo<CarExportDTO>(mapper.ConfigurationProvider)
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(CarExportDTO[]), new XmlRootAttribute("cars"));
        
            XmlSerializerNamespaces ns=new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using TextWriter writer = new StringWriter();
            serializer.Serialize(writer, cars, ns);

            return writer.ToString().TrimEnd();
           

        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BWMExport[]), new XmlRootAttribute("cars"));

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            Mapper mapper = GetMapper();

            var cars = context.Cars.Where(x => x.Make == "BMW").OrderBy(x => x.Model).ThenByDescending(x => x.TraveledDistance)
                .ProjectTo<BWMExport>(mapper.ConfigurationProvider)
                .ToArray();

            using TextWriter writer = new StringWriter();

            serializer.Serialize(writer, cars, ns);

            return writer.ToString();
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            Mapper mapper = GetMapper();
            var sup=context.Suppliers.Where(x=>!x.IsImporter)
                .ProjectTo<LocalSuppliersExport>(mapper.ConfigurationProvider)
                .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(LocalSuppliersExport[]), new XmlRootAttribute("suppliers"));

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using StringWriter writer = new StringWriter();

            serializer.Serialize(writer, sup, ns);

            return writer.ToString().TrimEnd();
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars=context.Cars.Select(x => new CarsWithPartsExport()
            {
                Make = x.Make,
                Model = x.Model,
                TraveledDistance = x.TraveledDistance,
                Parts = x.PartsCars.Select(y => new PartExport()
                {
                    Name = y.Part.Name,
                    Price = y.Part.Price,
                }).OrderByDescending(x=>x.Price).ToArray()
            }).OrderByDescending(x=>x.TraveledDistance)
            .ThenBy(x=>x.Model)
            .Take(5)
            .ToArray();

            return XMLSerializer(cars, "cars");
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            //var customers=context.Customers
            //    .Where(x=>x.Sales.Any())
            //    .Select(x=>new CustomersExport()
            //    {
            //        Name=x.Name,
            //        CarsBought=x.Sales.Count,
            //        IsYoung=x.IsYoungDriver,
            //        MoneyParts = x.Sales.Select(z=>z.Car.PartsCars.Select(y=>y.Part.Price).ToList()).ToList()
            //        ,
            //    })
            //    .ToArray().OrderByDescending(x => x.MoneySpent).ToArray();

            //return XMLSerializer(customers, "customers");

            var temp = context.Customers
                .Where(c => c.Sales.Any())
                .Select(c => new
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    SalesInfo = c.Sales.Select(s => new
                    {
                        Prices = c.IsYoungDriver
                                ? s.Car.PartsCars.Sum(pc => Math.Round((double)pc.Part.Price * 0.95, 2))
                                : s.Car.PartsCars.Sum(pc => (double)pc.Part.Price)
                    })
                        .ToArray(),
                })
                .ToArray();


            CustomersExport[] totalSales = temp.OrderByDescending(x => x.SalesInfo.Sum(y => y.Prices))
                .Select(x => new CustomersExport()
                {
                    Name = x.FullName,
                   CarsBought = x.BoughtCars,
                    MoneySpent = x.SalesInfo.Sum(y => (decimal)y.Prices)
                })
                .ToArray();

            return XMLSerializer(totalSales, "customers");

        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            ExportSaleAppliedDiscountDTO[] sales = context.Sales
                .Select(s => new ExportSaleAppliedDiscountDTO()
                {
                    Car = new ExportCarWithAttrDTO()
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance
                    },
                    Discount = (int)s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartsCars.Sum(p => p.Part.Price),
                    PriceWithDiscount =
                        Math.Round((double)(s.Car.PartsCars
                            .Sum(p => p.Part.Price) * (1 - (s.Discount / 100))), 4)
                })
                .ToArray();

            return XMLSerializer(sales, "sales");
        }

        private static string XMLSerializer<T>(T[] obj, string root)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T[]), new XmlRootAttribute(root));

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using StringWriter writer = new StringWriter();

            serializer.Serialize(writer, obj, ns);

            return writer.ToString().TrimEnd();
        }

        private static Mapper GetMapper()
        {
            var cfg = new MapperConfiguration(c => c.AddProfile<CarDealerProfile>());
            return new Mapper(cfg);
        }
    }
}