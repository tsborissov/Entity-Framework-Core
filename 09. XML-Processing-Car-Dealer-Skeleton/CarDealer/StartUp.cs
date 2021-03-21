using AutoMapper;
using CarDealer.Data;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var dbContext = new CarDealerContext();
            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();

            //var suppliersInputXml = File.ReadAllText("../../../Datasets/suppliers.xml");
            //var partsInputXml = File.ReadAllText("../../../Datasets/parts.xml");
            //var carsInputXml = File.ReadAllText("../../../Datasets/cars.xml");
            //var customersInputXml = File.ReadAllText("../../../Datasets/customers.xml");
            //var salesInputXml = File.ReadAllText("../../../Datasets/sales.xml");

            //ImportSuppliers(dbContext, suppliersInputXml);
            //ImportParts(dbContext, partsInputXml);
            //ImportCars(dbContext, carsInputXml);
            //ImportCustomers(dbContext, customersInputXml);
            //ImportSales(dbContext, salesInputXml);

            //var result = GetCarsWithDistance(dbContext);
            //GetCarsFromMakeBmw(dbContext);
            //GetLocalSuppliers(dbContext);
            //GetCarsWithTheirListOfParts(dbContext);
            //GetTotalSalesByCustomer(dbContext);
            var result = GetSalesWithAppliedDiscount(dbContext);

            System.Console.WriteLine(result);
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var dtoSales = context.Sales
                .Select(x => new SaleWithDiscountExportModel
                {
                    Car = new CarSaleExportModel
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance
                    },
                    Discount = x.Discount,
                    CustomerName = x.Customer.Name,
                    Price = x.Car.PartCars.Sum(x => x.Part.Price),
                    PriceWithDiscount = x.Car.PartCars.Sum(x => x.Part.Price) - x.Car.PartCars.Sum(x => x.Part.Price) * x.Discount / 100m
                })
                .ToArray();

            var xmlRoot = new XmlRootAttribute("sales");
            var serializer = new XmlSerializer(typeof(SaleWithDiscountExportModel[]), xmlRoot);

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var result = new StringWriter();
            serializer.Serialize(result, dtoSales, ns);

            return result.ToString();
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var dtoCustomers = context.Customers
                .Where(x => x.Sales.Any())
                .Select(x => new CustomerTotalSalesExportModel
                {
                    Name = x.Name,
                    BoughtCars = x.Sales.Count(),
                    SpentMoney = x.Sales
                    .Select(s => s.Car)
                    .SelectMany(p => p.PartCars)
                    .Sum(p => p.Part.Price)
                })
                .OrderByDescending(x => x.SpentMoney)
                .ToArray();

            var xmlRoot = new XmlRootAttribute("customers");
            var serializer = new XmlSerializer(typeof(CustomerTotalSalesExportModel[]), xmlRoot);

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var result = new StringWriter();
            serializer.Serialize(result, dtoCustomers, ns);

            return result.ToString();
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var dtoCarsWithParts = context.Cars
                .Select(x => new CarWithPartsExportModel
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance,
                    Parts = x.PartCars
                    .Select(p => new PartExportModel
                    {
                        Name = p.Part.Name,
                        Price = p.Part.Price
                    })
                    .OrderByDescending(x => x.Price)
                    .ToArray()
                })
                .OrderByDescending(x => x.TravelledDistance)
                .ThenBy(x => x.Model)
                .Take(5)
                .ToArray();

            var xmlRoot = new XmlRootAttribute("cars");
            var serializer = new XmlSerializer(typeof(CarWithPartsExportModel[]), xmlRoot);

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var result = new StringWriter();
            serializer.Serialize(result, dtoCarsWithParts, ns);

            return result.ToString();
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var localSuppliers = context.Suppliers
                .Where(x => !x.IsImporter)
                .Select(x => new LocalSupplierExportModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count
                })
                .ToArray();

            var xmlRoot = new XmlRootAttribute("suppliers");
            var serializer = new XmlSerializer(typeof(LocalSupplierExportModel[]), xmlRoot);

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var result = new StringWriter();
            serializer.Serialize(result, localSuppliers, ns);

            return result.ToString();
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var dtoTargetCars = context.Cars
                .Where(x => x.Make == "BMW")
                .Select(x => new CarFromMakeBmwExportModel
                {
                    Id = x.Id,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .ToArray();

            var xmlRoot = new XmlRootAttribute("cars");
            var serializer = new XmlSerializer(typeof(CarFromMakeBmwExportModel[]), xmlRoot);

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var result = new StringWriter();
            serializer.Serialize(result, dtoTargetCars, ns);

            return result.ToString();
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var dtoCars = context.Cars
                .Where(x => x.TravelledDistance > 2000000)
                .Select(x => new CarWithDistanceExportModel
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                .Take(10)
                .ToArray();

            var xmlRoot = new XmlRootAttribute("cars");
            var serializer = new XmlSerializer(typeof(CarWithDistanceExportModel[]), xmlRoot);
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var result = new StringWriter();
            serializer.Serialize(result, dtoCars, ns);

            return result.ToString();
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var validCarIds = context.Cars.Select(x => x.Id).ToArray();
            var validCustomerIds = context.Customers.Select(x => x.Id).ToArray();

            var xmlRoot = new XmlRootAttribute("Sales");
            var serializer = new XmlSerializer(typeof(SaleImportModel[]), xmlRoot);
            var inputXmlReader = new StringReader(inputXml);
            var dtoSales = (SaleImportModel[])serializer.Deserialize(inputXmlReader);

            var mapper = InitializeAutoMapper();

            var sales = mapper.Map<ICollection<Sale>>(dtoSales)
                .Where(x => validCarIds.Contains(x.CarId))
                .ToList();

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var xmlRoot = new XmlRootAttribute("Customers");
            var serializer = new XmlSerializer(typeof(CustomerImportModel[]), xmlRoot);
            var inputXmlReader = new StringReader(inputXml);
            var dtoCustomers = (CustomerImportModel[])serializer.Deserialize(inputXmlReader);

            var mapper = InitializeAutoMapper();

            var customers = mapper.Map<ICollection<Customer>>(dtoCustomers);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var validPartIds = context.Parts
                .Select(x => x.Id)
                .ToArray();

            var xmlRootAttribute = new XmlRootAttribute("Cars");
            var serializer = new XmlSerializer(typeof(CarImportModel[]), xmlRootAttribute);
            var xmlInputReader = new StringReader(inputXml);
            var dtoCars = (CarImportModel[])serializer.Deserialize(xmlInputReader);

            var cars = new List<Car>();

            foreach (var dtoCar in dtoCars)
            {
                var currentCar = new Car
                {
                    Make = dtoCar.Make,
                    Model = dtoCar.Model,
                    TravelledDistance = dtoCar.TraveledDistance
                };

                foreach (var dtoPartId in dtoCar.PartCars.Select(x => x.PartId).Distinct().Intersect(validPartIds))
                {
                    var currentPartCar = new PartCar
                    {
                        PartId = dtoPartId
                    };

                    currentCar.PartCars.Add(currentPartCar);
                }

                cars.Add(currentCar);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var validSupplierIds = context.Suppliers
                .Select(x => x.Id)
                .ToArray();

            var xmlRootAttribute = new XmlRootAttribute("Parts");
            var serializer = new XmlSerializer(typeof(PartImportModel[]), xmlRootAttribute);
            var xmlInputReader = new StringReader(inputXml);
            var dtoParts = (PartImportModel[])serializer.Deserialize(xmlInputReader);

            var mapper = InitializeAutoMapper();

            var parts = mapper.Map<ICollection<Part>>(dtoParts)
                .Where(x => validSupplierIds.Contains(x.SupplierId));

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}";
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var xmlRootAttribute = new XmlRootAttribute("Suppliers");
            var serializer = new XmlSerializer(typeof(SupplierImportModel[]), xmlRootAttribute);
            var xmlInputReader = new StringReader(inputXml);
            var dtoSuppliers = (SupplierImportModel[])serializer.Deserialize(xmlInputReader);

            var mapper = InitializeAutoMapper();
            var suppliers = mapper.Map<ICollection<Supplier>>(dtoSuppliers);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }

        private static IMapper InitializeAutoMapper()
        {
            var mapperConfiguration = new MapperConfiguration(c => 
            {
                c.AddProfile<CarDealerProfile>();
            });

            var mapper = mapperConfiguration.CreateMapper();

            return mapper;
        }
    }
}