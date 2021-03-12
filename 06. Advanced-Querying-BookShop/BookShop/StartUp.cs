namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            //var input = int.Parse(Console.ReadLine());

            //Console.WriteLine(result);

            var result = RemoveBooks(db);

            Console.WriteLine(result);
        }



        public static int RemoveBooks(BookShopContext context)
        {
            var targetBooks = context.Books
                .Where(x => x.Copies < 4200)
                .ToList();

            context.Books.RemoveRange(targetBooks);

            var result = context.SaveChanges();

            return targetBooks.Count;
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var targetBooks = context.Books
                .Where(x => x.ReleaseDate.Value.Year < 2010)
                .ToList();

            foreach (var book in targetBooks)
            {
                book.Price += 5;
            }

            context.SaveChanges();
       }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categoriesInfo = context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    CategoryBooks = c.CategoryBooks.Select(b => new
                    {
                        BookTitle = b.Book.Title,
                        BookReleaseDate = b.Book.ReleaseDate
                    })
                    .OrderByDescending(x => x.BookReleaseDate)
                    .Take(3)
                })
                .OrderBy(c => c.CategoryName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var category in categoriesInfo)
            {
                sb.AppendLine($"--{category.CategoryName}");

                foreach (var book in category.CategoryBooks)
                {
                    sb.AppendLine($"{book.BookTitle} ({(book.BookReleaseDate.Value.Year)})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories
                .Select(x => new
                {
                    CategoryName = x.Name,
                    CategoryProfit = x.CategoryBooks.Sum(b => (b.Book.Copies * b.Book.Price))
                })
                .OrderByDescending(x => x.CategoryProfit)
                .ThenBy(x => x.CategoryName)
                .ToList();

            var result = string.Join(Environment.NewLine,
                categories.Select(x => $"{x.CategoryName} ${x.CategoryProfit:F2}"));

            return result;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var autorsInfo = context.Authors
                .Select(a => new
                {
                    a.FirstName,
                    a.LastName,
                    BooksCopies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(x => x.BooksCopies)
                .ToList();

            var result = string.Join(Environment.NewLine, 
                autorsInfo.Select(x => $"{x.FirstName} {x.LastName} - {x.BooksCopies}"));

            return result;
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var booksCount = context.Books
                .Count(x => x.Title.Length > lengthCheck);

            return booksCount;
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                //.Where(x => x.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Where(x => EF.Functions.Like(x.Author.LastName, $"{input}%"))
                .OrderBy(x => x.BookId)
                .Select(x => $"{x.Title} ({x.Author.FirstName} {x.Author.LastName})")
                .ToList();


            var result = string.Join(Environment.NewLine, books);

            return result;
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var targetBooks = context.Books
                .Where(x => x.Title.ToLower().Contains(input.ToLower()))
                .Select(x => x.Title)
                .OrderBy(x => x)
                .ToList();

            var result = string.Join(Environment.NewLine, targetBooks);

            return result;
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var targetAuthors = context.Authors
                .Where(x => x.FirstName.EndsWith(input))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .Select(x => $"{x.FirstName} {x.LastName}")
                .ToList();

            var result = string.Join(Environment.NewLine, targetAuthors);

            return result;
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var targetDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(x => x.ReleaseDate.Value < targetDate)
                .OrderByDescending(x => x.ReleaseDate)
                .Select(x => $"{x.Title} - {x.EditionType} - ${x.Price:F2}") 
                .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var targetCategories = input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.ToLower())
                .ToList();

            var books = context.BooksCategories
                .Where(x => targetCategories.Contains(x.Category.Name.ToLower()))
                .Select(x =>  x.Book.Title)
                .OrderBy(x => x)
                .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }

        public static string GetBooksByCategory_v2(BookShopContext context, string input)
        {
            var targetCategories = input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.ToLower())
                .ToList();

            var books = context.Categories
                .Where(category => targetCategories.Contains(category.Name.ToLower()))
                .SelectMany(category => category.CategoryBooks
                    .Select(book => book.Book.Title))
                .OrderBy(title => title)
                .ToList();

            var result = string.Join(Environment.NewLine, books);

            return result;
        }

        public static string GetBooksByCategory_v3(BookShopContext context, string input)
        {
            var targetCategories = input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.ToLower())
                .ToList();

            var targetBooks = context.Books
                .Where(book => book.BookCategories
                    .Any(category => targetCategories
                    .Contains(category.Category.Name.ToLower())))
                .Select(book => book.Title)
                .OrderBy(title => title)
                .ToList();

            var result = string.Join(Environment.NewLine, targetBooks);

            return result;
        }

        public static string GetBooksByCategory_v4(BookShopContext context, string input)
        {
            var targetCategories = input
                .Split()
                .Select(x => x.ToLower())
                .ToArray();

            var books = context.Categories
                .Where(x => targetCategories.Contains(x.Name.ToLower()))
                .SelectMany(x => x.CategoryBooks
                    .Select(x => new
                    {
                        BookTitle = x.Book.Title
                    }))
                .OrderBy(x => x.BookTitle)
                .ToList();

            var result = string.Join(Environment.NewLine, books.Select(x => x.BookTitle));

            return result;
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(book => book.ReleaseDate.Value.Year != year)
                .Select(x => new
                {
                    x.BookId,
                    x.Title
                })
                .OrderBy(x => x.BookId)
                .ToList();

            var result = string.Join(Environment.NewLine, books.Select(x => x.Title));

            return result;
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(x => x.Price > 40)
                .Select(x => new
                {
                    x.Title,
                    x.Price
                })
                .OrderByDescending(x => x.Price)
                .ToList();

            var result = string.Join(Environment.NewLine, books.Select(x => $"{x.Title} - ${x.Price:F2}"));

            return result.ToString().TrimEnd();
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var goldenBooks = context.Books
                .Where(x => x.EditionType == EditionType.Gold && x.Copies < 5000)
                .Select(x => new
                {
                    x.BookId,
                    x.Title
                })
                .OrderBy(x => x.BookId)
                .ToList();

            var sb = new StringBuilder();

            var result = string.Join(Environment.NewLine, goldenBooks.Select(x => x.Title));

            return result;
        }


        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            string result = string.Empty;

            if (Enum.TryParse(command, true, out AgeRestriction commandEnum))
            {
                var books = context.Books
                .Where(books => books.AgeRestriction == commandEnum)
                .Select(book => book.Title)
                .OrderBy(title => title)
                .ToList();

                result = string.Join(Environment.NewLine, books);
            }

            return result.ToString().TrimEnd();
        }
    }
}

