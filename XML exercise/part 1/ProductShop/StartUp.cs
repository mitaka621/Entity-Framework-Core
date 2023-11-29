using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();
            var file = File.ReadAllText("../../../Datasets/categories-products.xml");

            Console.WriteLine(GetUsersWithProducts(context));
        }
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserDTO[]), new XmlRootAttribute("Users"));
            using StringReader xmlReader = new StringReader(inputXml);
            UserDTO[] usersDTO = (UserDTO[])xmlSerializer.Deserialize(xmlReader);

            List<User> users = new List<User>();

            foreach (var user in usersDTO)
            {
                users.Add(new User() { FirstName = user.FirstName, LastName = user.LastName, Age = user.Age });
            }
            context.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(DTOs.Import.ProductDTO[]), new XmlRootAttribute("Products"));
            using StringReader xmlReader = new StringReader(inputXml);
            DTOs.Import.ProductDTO[] usersDTO = (DTOs.Import.ProductDTO[])xmlSerializer.Deserialize(xmlReader);

            List<Product> users = new List<Product>();

            foreach (var user in usersDTO)
            {
                users.Add(new Product() { Name = user.Name, Price = user.Price, BuyerId = user.BuyerId, SellerId = user.SellerId });
            }
            context.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CategoryDTO[]), new XmlRootAttribute("Categories"));
            using StringReader xmlReader = new StringReader(inputXml);
            CategoryDTO[] usersDTO = (CategoryDTO[])xmlSerializer.Deserialize(xmlReader);

            List<Category> users = new List<Category>();

            foreach (var user in usersDTO)
            {
                users.Add(new Category() { Name = user.Name });
            }
            context.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CategorieProductDTO[]), new XmlRootAttribute("CategoryProducts"));

            using StringReader stringReader = new StringReader(inputXml);


            var categoriesProductsDTO = (CategorieProductDTO[])serializer.Deserialize(stringReader)!;

            List<CategoryProduct> categoryProducts = new List<CategoryProduct>();

            foreach (var item in categoriesProductsDTO)
            {
                if (context.Products.Any(p => p.Id == item.ProductId) && context.Categories.Any(x => x.Id == item.CategoryId))
                    categoryProducts.Add(new CategoryProduct() { CategoryId = item.CategoryId, ProductId = item.ProductId });
            }
            context.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var productsInRange = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .Select(p => new ExportProductsInRangeDto()
                {
                    Price = p.Price,
                    Name = p.Name,
                    Buyer = p.Buyer.FirstName + " " + p.Buyer.LastName
                })
                .ToArray();


            return XMLSerializer(productsInRange, "Products");
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var mapper = GetMapper();

            var users = context
            .Users
            .Where(u => u.ProductsSold.Any())
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Take(5)
            .ProjectTo<ExportUserDTO>(mapper.ConfigurationProvider)
            .ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(ExportUserDTO[]), new XmlRootAttribute("Users"));

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();
            using StringWriter stringWriter = new StringWriter(sb);

            serializer.Serialize(stringWriter, users, ns);

            return sb.ToString().TrimEnd();
        }
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var mapper = GetMapper();
           var categories= context.Categories
                .OrderByDescending(x=>x.CategoryProducts.Count)
                .ThenBy(x=>x.CategoryProducts.Sum(z=>z.Product.Price))
                .ProjectTo<ExportedCategory>(mapper.ConfigurationProvider).ToArray();

            XmlSerializer serializer = new XmlSerializer(typeof(ExportedCategory[]), new XmlRootAttribute("Categories"));

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using StringWriter writer = new StringWriter();
            serializer.Serialize(writer, categories, ns);
            return writer.ToString().TrimEnd();
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var mapper = GetMapper();

            var users=context.Users
                .Where(x => x.ProductsSold.Any())
                .Select(x=>new ExportedUserDto()
                {
                    FirstName=x.FirstName,
                    LastName=x.LastName,
                    Age=x.Age,
                    SoldProducts=new SoldProducts()
                    {
                        Count=x.ProductsSold.Count,
                        Products=x.ProductsSold.Select(y=>new ProductExportDTO()
                        {
                            Name=y.Name,
                            Price=y.Price,
                        })
                        .OrderByDescending(y=>y.Price)
                        .ToArray(),
                    }
                })
                .OrderByDescending(x => x.SoldProducts.Count)
                .Take(10)                
                .ToArray();
            var final = new AllUsersDto()
            {
                UsersCount = context.Users
                .Where(x => x.ProductsSold.Any()).Count(),
                Users = users
            };



            XmlSerializer serializer = new XmlSerializer(typeof(AllUsersDto), new XmlRootAttribute("Users"));

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            using StringWriter writer = new StringWriter();

            serializer.Serialize(writer, final, ns);

            return writer.ToString().TrimEnd();
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
            var cfg = new MapperConfiguration(c => c.AddProfile<ProductShopProfile>());
            return new Mapper(cfg);
        }
    }
}