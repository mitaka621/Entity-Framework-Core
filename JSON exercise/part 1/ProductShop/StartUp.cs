using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;
using System.Diagnostics;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();

            string usersJSON = File.ReadAllText("../../../Datasets/categories-products.json");

            Console.WriteLine(ImportCategoryProducts(context, usersJSON));
        }
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert.DeserializeObject<User[]>(inputJson);
            context.Users.AddRange(users);
            context.SaveChanges();
            return $"Successfully imported {users.Length}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<Product[]>(inputJson);
            context.Products.AddRange(products);
            context.SaveChanges();
            return $"Successfully imported {products.Length}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert.DeserializeObject<Category[]>(inputJson)
                .Where(x => x.Name != null).ToArray();
            context.Categories.AddRange(categories);
            context.SaveChanges();
            return $"Successfully imported {categories.Length}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoryProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);

            context.CategoriesProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Length}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products.Select(x => new
            {
                name = x.Name,
                price = x.Price,
                seller = x.Seller.FirstName + " " + x.Seller.LastName
            }).Where(x => x.price >= 500 && x.price <= 1000)
             .OrderBy(x => x.price);

            return JsonConvert.SerializeObject(products, Formatting.Indented);
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users.Where(x => x.ProductsSold.Any(z => z.Buyer != null)).Select(x => new
            {
                firstName = x.FirstName,
                lastName = x.LastName,
                soldProducts = x.ProductsSold.Where(y => y.Buyer != null).Select(y => new
                {
                    name = y.Name,
                    price = y.Price,
                    buyerFirstName = y.Buyer.FirstName,
                    buyerLastName = y.Buyer.LastName
                })
            }).OrderBy(x => x.lastName).ThenBy(x => x.firstName);

            return JsonConvert.SerializeObject(users, Formatting.Indented);
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {

            var categories = context.Categories.OrderByDescending(x => x.CategoriesProducts.Count).Select(x => new
            {
                category = x.Name,
                productsCount = x.CategoriesProducts.Count,
                averagePrice = x.CategoriesProducts.Count != 0 ?
                    $"{x.CategoriesProducts.Average(y => y.Product.Price):F2}"
                    : "0.00",
                totalRevenue = x.CategoriesProducts.Count != 0 ?
                $"{x.CategoriesProducts.Sum(y => y.Product.Price):F2}"
                : "0.00"
            });

            return JsonConvert.SerializeObject(categories, Formatting.Indented);
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users.Where(x => x.ProductsSold.Any(x => x.BuyerId != null)).Select(x => new
            {
                firstName = x.FirstName,
                lastName = x.LastName,
                age = x.Age,
                soldProducts = new
                {
                    count = x.ProductsSold.Where(x => x.BuyerId != null).Count(),
                    products = x.ProductsSold.Where(x => x.BuyerId != null).Select(x => new
                    {
                        name = x.Name,
                        price = x.Price
                    }).ToList(),
                },
            }).OrderByDescending(x => x.soldProducts.count).ToArray();

            return JsonConvert.SerializeObject(new 
            {
                usersCount = users.Length,
                users
            },
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore
                });
        }
    }
}