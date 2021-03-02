using P01_StudentSystem.Data;
using P01_StudentSystem.Data.Enums;
using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var db = new StudentSystemContext();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            //var resource = new Resource()
            //{
            //    Name = "Entity Framework Core",
            //    Url = "https://docs.microsoft.com/en-us/ef/core/",
            //    ResourceType = ResourceType.Other,
            //    CourseId = null
            //};

            //db.Resources.Add(resource);

            //db.SaveChanges();
        }
    }
}
