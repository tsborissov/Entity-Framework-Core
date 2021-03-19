using AutoMapper;
using ProductShop.Data;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using ProductShop.Dtos.Export;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //var usersInputXml = File.ReadAllText("../../../Datasets/users.xml");
            //var productsInputXml = File.ReadAllText("../../../Datasets/products.xml");
            //var categoriesInputXml = File.ReadAllText("../../../Datasets/categories.xml");
            //var categoriesProductsInputXml = File.ReadAllText("../../../Datasets/categories-products.xml");

            //ImportUsers(context, usersInputXml);
            //ImportProducts(context, productsInputXml);
            //ImportCategories(context, categoriesInputXml);
            //ImportCategoryProducts(context, categoriesProductsInputXml);

            //GetProductsInRange(context);
            //GetSoldProducts(context);

            var result = GetUsersWithProducts(context);

            System.Console.WriteLine(result);
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .ToArray()
                .Where(x => x.ProductsSold.Any())
                .Select(u => new UserExportModel
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new ProductsCountExportModel
                    {
                        SoldProductsCount = u.ProductsSold.Count,
                        SoldProducts = u.ProductsSold
                        .Select(p => new ProductExportModel
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .OrderByDescending(x => x.Price)
                        .ToArray()
                    }
                })
                .OrderByDescending(x => x.SoldProducts.SoldProductsCount)
                .Take(10)
                .ToArray();

            var result = new UsersCountExportModel
            {
                UsersCount = context.Users.Count(x => x.ProductsSold.Any()),
                Users = users
            };

            var xmlRootAttribute = new XmlRootAttribute("Users");
            var serializer = new XmlSerializer(typeof(UsersCountExportModel), xmlRootAttribute);
            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(textWriter, result, ns);

            return textWriter.ToString();
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(x => new CategoryByProductExportModel
                {
                    Name = x.Name,
                    Count = x.CategoryProducts.Count,
                    AveragePrice = x.CategoryProducts.Average(p => p.Product.Price),
                    TotalRevenue = x.CategoryProducts.Sum(p => p.Product.Price)
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.TotalRevenue)
                .ToArray();

            var xmlRootAttribute = new XmlRootAttribute("Categories");
            var serializer = new XmlSerializer(typeof(CategoryByProductExportModel[]), xmlRootAttribute);
            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(textWriter, categories, ns);

            return textWriter.ToString();
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Count > 0)
                .Select(x => new UserSoldProductsExportModel
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold
                    .Select(p => new SoldProductExportModel 
                    {
                        Name = p.Name,
                        Price = p.Price
                    })
                    .ToArray()
                })
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Take(5)
                .ToArray();

            var xmlRootAttribute = new XmlRootAttribute("Users");
            var serializer = new XmlSerializer(typeof(UserSoldProductsExportModel[]), xmlRootAttribute);
            var textWriter = new StringWriter();
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            serializer.Serialize(textWriter, users, ns);

            return textWriter.ToString();
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var dtoExportProducts = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(x => new ProductWithBuyerExportModel
                {
                    Name = x.Name,
                    Price = x.Price,
                    BuyerFullName = x.Buyer.FirstName + ' ' + x.Buyer.LastName
                })
                .OrderBy(p => p.Price)
                .Take(10)
                .ToArray();

            var xmlRootAttribute = new XmlRootAttribute("Products");
            var serializer = new XmlSerializer(typeof(ProductWithBuyerExportModel[]), xmlRootAttribute);
            var textWriter = new StringWriter();

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            serializer.Serialize(textWriter, dtoExportProducts, ns);


            return textWriter.ToString();
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var xmlRootAttribute = new XmlRootAttribute("CategoryProducts");
            var serializer = new XmlSerializer(typeof(CategoryProductImportModel[]), xmlRootAttribute);

            var validCategoryIds = context.Categories
                .Select(x => x.Id)
                .Distinct()
                .ToList();

            var validProductIds = context.Products
                .Select(x => x.Id)
                .Distinct()
                .ToList();
               

            var inputXmlReader = new StringReader(inputXml);
            var dtoCategoryProducts = (CategoryProductImportModel[])serializer.Deserialize(inputXmlReader);

            var mapper = InitializeAutoMapper();
            var categoryProducts = mapper.Map<ICollection<CategoryProduct>>(dtoCategoryProducts)
                .Where(x => validCategoryIds.Contains(x.CategoryId) && validProductIds.Contains(x.ProductId))
                .ToList();

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var xmlRootAttribute = new XmlRootAttribute("Categories");
            var serializer = new XmlSerializer(typeof(CategoryImportModel[]), xmlRootAttribute);

            var inputXmlReader = new StringReader(inputXml);
            var dtoCategories = (CategoryImportModel[])serializer.Deserialize(inputXmlReader);

            var mapper = InitializeAutoMapper();
            var categories = mapper.Map<ICollection<Category>>(dtoCategories);

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}"; ;
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var xmlRootAttribute = new XmlRootAttribute("Products");
            var serializer = new XmlSerializer(typeof(ProductImportModel[]), xmlRootAttribute);

            var inputXmlReader = new StringReader(inputXml);
            var dtoProducts = (ProductImportModel[])serializer.Deserialize(inputXmlReader);

            var mapper = InitializeAutoMapper();
            var products = mapper.Map<ICollection<Product>>(dtoProducts);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}"; ;
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var rootAttribute = new XmlRootAttribute("Users");
            var serializer = new XmlSerializer(typeof(UserImportModel[]), rootAttribute);

            var inputXmlReader = new StringReader(inputXml);
            var usersDtos = (UserImportModel[])serializer.Deserialize(inputXmlReader);

            var mapper = InitializeAutoMapper();
            var users = mapper.Map<ICollection<User>>(usersDtos);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}"; ;
        }

        private static IMapper InitializeAutoMapper()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            var mapper = mapperConfiguration.CreateMapper();

            return mapper;
        }
    }
}