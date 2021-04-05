namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var data = context.Authors.ToArray()
                .Select(x => new
                {
                    AuthorName = x.FirstName + ' ' + x.LastName,
                    Books = x.AuthorsBooks
                    .OrderByDescending(p => p.Book.Price)
                    .Select(b => new 
                    {
                        BookName = b.Book.Name,
                        BookPrice = b.Book.Price.ToString("F2")
                    })
                    .ToArray()
                })
                .OrderByDescending(c => c.Books.Count())
                .ThenBy(n => n.AuthorName);

            var result = JsonConvert.SerializeObject(data, Formatting.Indented);

            return result;
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var data = context.Books.ToArray()
                .Where(x => x.PublishedOn < date && x.Genre == Genre.Science)
                .OrderByDescending(x => x.Pages)
                .ThenByDescending(x => x.PublishedOn)
                .Take(10)
                .Select(b => new BookXmlExportModel
                {
                    Pages = b.Pages,
                    Name = b.Name,
                    Date = b.PublishedOn.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                })
                .ToArray();


            var xmlRootAttribute = new XmlRootAttribute("Books");
            var serializer = new XmlSerializer(typeof(BookXmlExportModel[]), xmlRootAttribute);
            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(textWriter, data, ns);

            return textWriter.ToString();
        }
    }
}