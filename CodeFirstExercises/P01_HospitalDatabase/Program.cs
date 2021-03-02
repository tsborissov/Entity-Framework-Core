using P01_HospitalDatabase.Data;
using System;

namespace P01_HospitalDatabase
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var db = new HospitalContext();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }
    }
}
