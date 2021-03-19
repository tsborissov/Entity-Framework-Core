using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType]
    public class UsersCountExportModel
    {
        [XmlElement("count")]
        public int UsersCount { get; set; }

        [XmlArray("users")]
        public UserExportModel[] Users { get; set; }
    }

    [XmlType("User")]
    public class UserExportModel
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("age")]
        public int? Age { get; set; }

        [XmlElement("SoldProducts")]
        public ProductsCountExportModel SoldProducts { get; set; }
    }

    [XmlType]
    public class ProductsCountExportModel
    {
        [XmlElement("count")]
        public int SoldProductsCount { get; set; }

        [XmlArray("products")]
        public ProductExportModel[] SoldProducts { get; set; }
    }

    [XmlType("Product")]
    public class ProductExportModel
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}

/*
  <Users>
  <count>54</count>
  <users>
    <User>
      <firstName>Cathee</firstName>
      <lastName>Rallings</lastName>
      <age>33</age>
      <SoldProducts>
        <count>9</count>
        <products>
          <Product>
            <name>Fair Foundation SPF 15</name>
            <price>1394.24</price>
          </Product>
          <Product>
 */