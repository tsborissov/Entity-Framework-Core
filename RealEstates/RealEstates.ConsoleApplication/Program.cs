using Microsoft.EntityFrameworkCore;
using RealEstates.Data;
using System;

namespace RealEstates.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbContext = new RealEstatesContext();
            dbContext.Database.Migrate();
        }
    }
}
