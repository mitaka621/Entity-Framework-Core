namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System.Globalization;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);
            Console.WriteLine(RemoveBooks(db));
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            
            if (!Enum.TryParse<AgeRestriction>(command,true,out var result))
            {
                return "";
                
            }
            var books = context.Books.Where(x => x.AgeRestriction == result).Select(x=>x.Title).AsNoTracking().OrderBy(b=>b).ToList();

            
            return string.Join("\n", books);
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var books=context.Books.Where(x=>x.EditionType==EditionType.Gold&& x.Copies<5000).Select(x=>x.Title).AsNoTracking().ToList();

            return string.Join("\n", books);
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books.Where(x => x.Price > 40).OrderByDescending(x => x.Price).Select(x => new { x.Title, x.Price }).ToList();

            return string.Join("\n", books.Select(x=>$"{x.Title} - ${x.Price:F2}"));
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books.Where(x => x.ReleaseDate.Value.Year!=year).Select(x => x.Title).AsNoTracking().ToList();

            return string.Join("\n", books);
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categories = input.Split(" ").Select(x=>x.ToLower()).ToList();
            var books = context.BooksCategories.Include(x=>x.Book).Where(x => categories.Any(y => y == x.Category.Name.ToLower())).OrderBy(x=>x.Book.Title).ToList();

            
            return string.Join("\n", books.Select(x=>x.Book.Title));
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var parsedDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
           

            var books=context.Books.Where(x=>x.ReleaseDate< parsedDate)               
                .Select(x => new {x.Title, x.EditionType, x.Price, x.ReleaseDate})
                .OrderByDescending(x => x.ReleaseDate);

            return string.Join(Environment.NewLine, books
                .Select(x=>$"{x.Title} - {x.EditionType} - ${x.Price:f2}"));

        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var books=context.Authors.Where(x=>x.FirstName.EndsWith(input)).Select(x=>new {FullName= x.FirstName + " " + x.LastName}).OrderBy(x=>x.FullName).ToList();

            return string.Join(Environment.NewLine, books.Select(x =>x.FullName ));
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books.Where(x => x.Title.ToLower().Contains(input.ToLower())).Select(x => x.Title).OrderBy(x => x);

            return string.Join(Environment.NewLine, books);
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var authors = context.Authors.Where(x => x.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(x => new
                {
                    Titles=x.Books.Select(y=>y.Title),
                    FullName = x.FirstName + " " + x.LastName,
                });
            StringBuilder sb = new StringBuilder();
            foreach (var author in authors)
            {
                foreach (var title in author.Titles)
                {
                    sb.AppendLine($"{title} ({author.FullName})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var titlesCount = context.Books.Where(x => x.Title.Length > lengthCheck)
                .Count();
            return titlesCount;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors.Select(x => new
            {
                FullName = x.FirstName + " " + x.LastName,
                BookCount = x.Books.Sum(y=>y.Copies)
            }).OrderByDescending(x => x.BookCount);

            return String.Join(Environment.NewLine, authors.Select(x => $"{x.FullName} - {x.BookCount}"));
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories.Select(x => new
            {
                x.Name,
                Profit = x.CategoryBooks.Sum(x => x.Book.Copies * x.Book.Price)
            })
            .OrderByDescending(x => x.Profit)
            .ThenBy(x => x.Name)
            .AsNoTracking();
            return String.Join(Environment.NewLine, categories
                .Select(x => $"{x.Name} ${x.Profit:f2}"));
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categories = context.Categories.Select(x => new
            {
                x.Name,
                Books = x.CategoryBooks.OrderByDescending(x => x.Book.ReleaseDate).Select(y => new
                {
                    y.Book.Title,
                    y.Book.ReleaseDate.Value.Year,
                }).Take(3)
            })
            .OrderBy(x => x.Name)
            .AsNoTracking();

            StringBuilder sb = new StringBuilder();

            foreach (var item in categories)
            {
                sb.AppendLine($"--{item.Name}");
                foreach (var book in item.Books)
                {
                    sb.AppendLine($"{book.Title} ({book.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }
        public static void IncreasePrices(BookShopContext context)
        {
            var b= context.Books.Where(x => x.ReleaseDate.Value.Year < 2010);

            foreach (var book in b)
            {
                book.Price += 5;
            }
            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var b = context.Books.Where(x => x.Copies < 4200).ToList();

            foreach (var item in b)
            {
               context.Remove(item);
            }
            context.SaveChanges ();
            return b.Count;
        }
    }
}


