using CarDealer.Data;
using CarDealer.DTOs;
using CarDealer.Models;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using System.Xml.Schema;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();
            string file = File.ReadAllText("../../../Datasets/sales.json");
            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var supp = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

            context.Suppliers.AddRange(supp);
            context.SaveChanges();
            return $"Successfully imported {supp.Length}.";
        }
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var Part = JsonConvert.DeserializeObject<Part[]>(inputJson).Where(x=>context.Suppliers.Any(y=>y.Id==x.SupplierId)).ToArray();

            context.Parts.AddRange(Part);
            context.SaveChanges();
            return $"Successfully imported {Part.Length}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var cars = JsonConvert.DeserializeObject<CarDTO[]>(inputJson);

            foreach (var item in cars)
            {
                Car tempCar = new Car()
                {
                    Make = item.Make,
                    Model = item.Model,
                    TraveledDistance = item.TraveledDistance,
                    
                };
                foreach (var part in item.PartsCars)
                {
                    if (context.Parts.Any(p => p.Id == part))
                    {
                        tempCar.PartsCars.Add(new PartCar() { PartId = part });
                    }
                }

                context.Cars.Add(tempCar);
            }


            context.SaveChanges();

            return $"Successfully imported {cars.Length}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var Customer = JsonConvert.DeserializeObject<Customer[]>(inputJson);

            context.Customers.AddRange(Customer);
            context.SaveChanges();
            return $"Successfully imported {Customer.Length}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var Sale = JsonConvert.DeserializeObject<Sale[]>(inputJson);

            context.Sales.AddRange(Sale);
            context.SaveChanges();
            return $"Successfully imported {Sale.Length}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
           var customers= context.Customers.Select(x => new
            {
                Name = x.Name,
                BirthDate = x.BirthDate,
                IsYoungDriver = x.IsYoungDriver
            })
                .OrderBy(x => x.BirthDate)
                .ThenBy(x => x.IsYoungDriver);
            List<object> nice= new List<object>();
            foreach (var item in customers)
            {
                nice.Add(new
                {
                    Name=item.Name,
                    BirthDate = item.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver=item.IsYoungDriver
                });
            }
            return JsonConvert.SerializeObject(nice, Formatting.Indented) ;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars=context.Cars.Where(x => x.Make == "Toyota").OrderBy(x => x.Model).ThenByDescending(x => x.TraveledDistance).Select(x => new
            {
                Id = x.Id,
                x.Make,
                x.Model,
                x.TraveledDistance
            });

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var sup=context.Suppliers.Where(x => !x.IsImporter).Select(x => new
            {
                x.Id,
                x.Name,
                PartsCount = x.Parts.Count
            });
            return JsonConvert.SerializeObject(sup, Formatting.Indented);

        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars=context.Cars.Select(x => new
            {
                car = new
                {
                    x.Make,
                    x.Model,
                    x.TraveledDistance
                },
                parts = x.PartsCars.Select(y => new
                {
                    y.Part.Name,
                    Price = $"{y.Part.Price:F2}"
                }).ToArray()
            });

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var result = context.Customers
               .Where(x => x.Sales.Count > 0)
               .Select(x => new
               {
                   fullName = x.Name,
                   boughtCars = x.Sales.Count,
                   spentMoney = x.Sales.Sum(x => x.Car.PartsCars.Sum(x => x.Part.Price)),
               })
               .OrderByDescending(x => x.spentMoney)
               .ThenByDescending(x => x.boughtCars)
               .ToArray();

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);

           

            return json;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var result = context.Sales
               .Take(10)
               .Select(x => new
               {
                   car = new
                   {
                       x.Car.Make,
                       x.Car.Model,
                       x.Car.TraveledDistance
                   },
                   customerName = x.Customer.Name,
                   discount = x.Discount.ToString("f2"),
                   price = x.Car.PartsCars.Sum(x => x.Part.Price).ToString("f2"),
                   priceWithDiscount = (x.Car.PartsCars.Sum(x => x.Part.Price) - x.Car.PartsCars.Sum(x => x.Part.Price) * (x.Discount / 100)).ToString("f2")
               })
               .ToArray();

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);

           

            return json.Trim();
        }
    }
}