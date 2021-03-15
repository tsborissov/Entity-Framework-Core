using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.DTOs;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //var usersJsonFile = "../../../Datasets/users.json";
            //var productsJsonFile = "../../../Datasets/products.json";
            //var categoriesJsonFile = "../../../Datasets/categories.json";
            //var categoriesProductsJsonFile = "../../../Datasets/categories-products.json";

            //var usersInputJson = File.ReadAllText(usersJsonFile);
            //var productsInputJson = File.ReadAllText(productsJsonFile);
            //var categoriesInputJson = File.ReadAllText(categoriesJsonFile);
            //var categoriesProductsInputJson = File.ReadAllText(categoriesProductsJsonFile);

            //ImportUsers(context, usersInputJson);
            //ImportProducts(context, productsInputJson);
            //ImportCategories(context, categoriesInputJson);
            //ImportCategoryProducts(context, categoriesProductsInputJson);

            //GetProductsInRange(context);
            //GetSoldProducts(context);
            //GetCategoriesByProductsCount(context);
            //GetUsersWithProducts(context);

            //Console.WriteLine(result);
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Any(p => p.BuyerId != null))
                .Select(u => new
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new
                    {
                        Count = u.ProductsSold.Where(b => b.BuyerId != null).Count(),
                        Products = u.ProductsSold
                        .Where(x => x.BuyerId != null)
                        .Select(p => new
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .ToList()
                    }
                })
                .OrderByDescending(x => x.SoldProducts.Count)
                .ToList();

            var resultObject = new
            {
                UsersCount = users.Count(),
                Users = users
            };


            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializerSettings.Formatting = Formatting.Indented;
            serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            var jsonResult = JsonConvert.SerializeObject(resultObject, serializerSettings);

            return jsonResult;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(x => new 
                {
                    Category = x.Name,
                    ProductsCount = x.CategoryProducts.Count,
                    AveragePrice = (x.CategoryProducts.Sum(p => p.Product.Price) / x.CategoryProducts.Count).ToString("F2"),
                    TotalRevenue = x.CategoryProducts.Sum(p => p.Product.Price).ToString("F2")
                })
                .OrderByDescending(x => x.ProductsCount)
                .ToList();

            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializerSettings.Formatting = Formatting.Indented;

            var jsonResult = JsonConvert.SerializeObject(categories, serializerSettings);

            return jsonResult;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var sellers = context.Users
                .Where(s => s.ProductsSold.Any(p =>p.BuyerId != null))
                .Select(s => new
                {
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    SoldProducts = s.ProductsSold
                    .Where(x => x.BuyerId != null)
                    .Select(p => new
                    {
                        Name = p.Name,
                        Price = p.Price,
                        BuyerFirstName = p.Buyer.FirstName,
                        BuyerLastName = p.Buyer.LastName
                    })
                    .ToList()
                })
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToList();
            
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializerSettings.Formatting = Formatting.Indented;

            var jsonResult = JsonConvert.SerializeObject(sellers, serializerSettings);

            return jsonResult;
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(x => new
                {
                    Name = x.Name,
                    Price = x.Price,
                    Seller = String.Concat(x.Seller.FirstName + " ", x.Seller.LastName)
                })
                .OrderBy(p => p.Price)
                .ToList();

            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            serializerSettings.Formatting = Formatting.Indented;

            var jsonResult = JsonConvert.SerializeObject(products, serializerSettings);

            return jsonResult;
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var dtoCategoryProducts = JsonConvert.DeserializeObject<ICollection<CategoryProductInputModel>>(inputJson);

            var mapper = InitializeMapper();

            var categoryProducts = mapper.Map<IEnumerable<CategoryProduct>>(dtoCategoryProducts);

            context.AddRange(categoryProducts);

            var result = context.SaveChanges();

            return $"Successfully imported {result}"; ;
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var dtoCategories = JsonConvert
                .DeserializeObject<IEnumerable<CategoryInputModel>>(inputJson)
                .Where(x => x.Name != null)
                .ToList();

            
            var mapper = InitializeMapper();

            var categories = mapper.Map<IEnumerable<Category>>(dtoCategories);

            context.AddRange(categories);
            var categoriesCount = context.SaveChanges();

            return $"Successfully imported {categoriesCount}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var dtoProducts = JsonConvert.DeserializeObject<IEnumerable<ProductInputModel>>(inputJson);

            var mapper = InitializeMapper();

            var products = mapper.Map<IEnumerable<Product>>(dtoProducts);

            context.AddRange(products);
            var productsCount = context.SaveChanges();

            return $"Successfully imported {productsCount}";
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var dtoUsers = JsonConvert.DeserializeObject<IEnumerable<UserInputModel>>(inputJson);

            var mapper = InitializeMapper();

            var users = mapper.Map<IEnumerable<User>>(dtoUsers);

            context.AddRange(users);
            var usersAdded = context.SaveChanges();

            return $"Successfully imported {usersAdded}";
        }

        private static IMapper InitializeMapper()
        {
            var config = new MapperConfiguration(cfg =>
                cfg.AddProfile<ProductShopProfile>());
            var mapper = config.CreateMapper();

            return mapper;
        }
    }
}