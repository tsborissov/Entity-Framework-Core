namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using BookShop.Data.Models;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            var xmlRoot = new XmlRootAttribute("Books");
            var serializer = new XmlSerializer(typeof(BookXmlImportModel[]), xmlRoot);
            var inputXmlReader = new StringReader(xmlString);
            var dtoBooks = (BookXmlImportModel[])serializer.Deserialize(inputXmlReader);

            var sb = new StringBuilder();

            var validBooks = new List<Book>();

            foreach (var currentDtoBook in dtoBooks)
            {
                if (!IsValid(currentDtoBook) ||
                    ! Enum.IsDefined(typeof(Genre), currentDtoBook.Genre) ||
                    !DateTime
                        .TryParseExact(currentDtoBook.PublishedOn,
                        "MM/dd/yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime currentBookDate))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }



                var book = new Book 
                {
                    Name = currentDtoBook.Name,
                    Genre = (Genre)currentDtoBook.Genre,
                    Price = currentDtoBook.Price,
                    Pages = currentDtoBook.Pages,
                    PublishedOn = currentBookDate
                };

                validBooks.Add(book);

                sb.AppendLine($"Successfully imported book {book.Name} for {book.Price:F2}.");
            }

            context.Books.AddRange(validBooks);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            var data = JsonConvert.DeserializeObject<IEnumerable<AuthorJsonImportModel>>(jsonString);

            var sb = new StringBuilder();

            foreach (var dtoAuthor in data)
            {
                if (!IsValid(dtoAuthor) ||
                    context.Authors.Any(x => x.Email == dtoAuthor.Email))
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                var currentAuthorBooks = new List<AuthorBook>();

                var existingBooksIds = context.Books.Select(x => x.Id).ToList();

                foreach (var dtoBook in dtoAuthor.Books)
                {
                    if (dtoBook.Id == null || !existingBooksIds.Contains(dtoBook.Id.Value))
                    {
                        continue;
                    }

                    currentAuthorBooks.Add(new AuthorBook 
                    {
                        BookId = dtoBook.Id.Value
                    });
                }

                if (!currentAuthorBooks.Any())
                {
                    sb.AppendLine("Invalid data!");
                    continue;
                }

                var author = new Author 
                {
                    FirstName = dtoAuthor.FirstName,
                    LastName = dtoAuthor.LastName,
                    Email = dtoAuthor.Email,
                    Phone = dtoAuthor.Phone,
                    AuthorsBooks = currentAuthorBooks
                };

                sb.AppendLine($"Successfully imported author - {author.FirstName + ' ' + author.LastName} with {currentAuthorBooks.Count} books.");

                context.Authors.Add(author);
                context.SaveChanges();
            }

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}