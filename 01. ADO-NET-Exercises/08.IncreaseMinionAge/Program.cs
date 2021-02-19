using Microsoft.Data.SqlClient;
using System;
using System.Linq;

namespace _08.IncreaseMinionAge
{
    class Program
    {
        static void Main(string[] args)
        {
            var minionsIds = Console.ReadLine().Split().Select(int.Parse).ToArray();

            var dbConnectionString = @"Server=(localdb)\MSSQLLocalDB; Database=MinionsDB; Integrated Security=true";

            var updateNameAgeQuery = @"UPDATE Minions
                                        SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1
                                       WHERE Id = @Id";

            var getMinionsNameAgeQuery = @"SELECT Name, Age FROM Minions";

            var dbConnection = new SqlConnection(dbConnectionString);
            dbConnection.Open();
            using(dbConnection)
            {
                foreach (var minionId in minionsIds)
                {
                    var updateNameAgeCommand = new SqlCommand(updateNameAgeQuery, dbConnection);
                    updateNameAgeCommand.Parameters.AddWithValue("@Id", minionId);
                    updateNameAgeCommand.ExecuteNonQuery();
                }

                var getMinionsNameAgeCommand = new SqlCommand(getMinionsNameAgeQuery, dbConnection);
                var dbReader = getMinionsNameAgeCommand.ExecuteReader();

                using(dbReader)
                {
                    while (dbReader.Read())
                    {
                        Console.WriteLine($"{(string)dbReader["Name"]} {(int)dbReader["Age"]}");
                    }
                }
            }
        }
    }
}
