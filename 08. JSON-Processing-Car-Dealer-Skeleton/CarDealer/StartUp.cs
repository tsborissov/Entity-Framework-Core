using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var dbContext = new CarDealerContext();

            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();

            //var suppliersJson = File.ReadAllText("../../../Datasets/suppliers.json");
            //var partsJson = File.ReadAllText("../../../Datasets/parts.json");
            //var carsJson = File.ReadAllText("../../../Datasets/cars.json");
            //var customersJson = File.ReadAllText("../../../Datasets/customers.json");
            //var salesJson = File.ReadAllText("../../../Datasets/sales.json");

            //ImportSuppliers(dbContext, suppliersJson);
            //ImportParts(dbContext, partsJson);
            //ImportCars(dbContext, carsJson);
            //ImportCustomers(dbContext, customersJson);
            //ImportSales(dbContext, salesJson);

            var result = GetCarsWithTheirListOfParts(dbContext);

            Console.WriteLine(result);
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new 
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    parts = c.PartCars
                    .Select(p => new
                    {
                        Name = p.Part.Name,
                        Price = p.Part.Price.ToString("F2")
                    })
                    .ToList()
                })
                .ToList();


            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Formatting = Formatting.Indented;

            var jsonResult = JsonConvert.SerializeObject(cars, jsonSerializerSettings);

            return jsonResult;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var targetSuppliers = context.Suppliers
                .Where(x => x.IsImporter == false)
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count
                })
                .ToList();

            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Formatting = Formatting.Indented;

            var jsonResult = JsonConvert.SerializeObject(targetSuppliers, jsonSettings);

            return jsonResult;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var targetCars = context.Cars
                .Where(x => x.Make == "Toyota")
                .Select(x => new 
                {
                    Id = x.Id,
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ToList();

            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Formatting = Formatting.Indented;

            var jsonResult = JsonConvert.SerializeObject(targetCars, jsonSerializerSettings);


            return jsonResult;
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .Select(x => new
                {
                    Name = x.Name,
                    BirthDate = x.BirthDate,
                    IsYoungDriver = x.IsYoungDriver
                })
                .OrderBy(x => x.BirthDate)
                .ThenBy(x => x.IsYoungDriver)
                .ToList();

            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Formatting = Formatting.Indented;
            jsonSerializerSettings.DateFormatString = "dd/MM/yyyy";

            var resultJson = JsonConvert.SerializeObject(customers, jsonSerializerSettings);

            return resultJson;
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var salesDto = JsonConvert.DeserializeObject<ICollection<SaleInputModel>>(inputJson);

            var mapper = InitializeAutoMapper();
            var sales = mapper.Map<ICollection<Sale>>(salesDto);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customersDto = JsonConvert.DeserializeObject<ICollection<CustomerInputModel>>(inputJson);

            var mapper = InitializeAutoMapper();

            var customers = mapper.Map<ICollection<Customer>>(customersDto);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var dtoCars = JsonConvert.DeserializeObject<ICollection<CarInputModel>>(inputJson);

            var cars = new List<Car>();

            foreach (var dtoCar in dtoCars)
            {
                var currentCar = new Car
                {
                    Make = dtoCar.Make,
                    Model = dtoCar.Model,
                    TravelledDistance = dtoCar.TravelledDistance
                };

                foreach (var dtoPart in dtoCar.PartsId.Distinct())
                {
                    var currentPartCar = new PartCar
                    {
                        PartId = dtoPart
                    };

                    currentCar.PartCars.Add(currentPartCar);
                }

                cars.Add(currentCar);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var validSupplierIds = context.Suppliers.Select(x => x.Id).Distinct().ToArray();

            var dtoParts = JsonConvert.DeserializeObject<ICollection<PartInputModel>>(inputJson)
                .Where(x => validSupplierIds.Contains(x.SupplierId))
                .ToList();

            var mapper = InitializeAutoMapper();
            var parts = mapper.Map<ICollection<Part>>(dtoParts);

            context.Parts.AddRange(parts);
            var resultCount = context.SaveChanges();

            return $"Successfully imported {resultCount}.";
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var dtoSuppliers = JsonConvert.DeserializeObject<ICollection<SupplierInputModel>>(inputJson);

            var mapper = InitializeAutoMapper();
            var suppliers = mapper.Map<ICollection<Supplier>>(dtoSuppliers);

            context.Suppliers.AddRange(suppliers);
            var suppliersCount = context.SaveChanges();
            

            return $"Successfully imported {suppliersCount}.";
        }

        private static IMapper InitializeAutoMapper()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
                cfg.AddProfile<CarDealerProfile>());
            var mapper = mapperConfig.CreateMapper();

            return mapper;
        }
    }
}