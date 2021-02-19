using Microsoft.Data.SqlClient;
using System;

namespace _09.IncreaseAgeStoredProcedure
{
    class Program
    {
        static void Main(string[] args)
        {
            var minionId = int.Parse(Console.ReadLine());

            var increaseAgeQuery = @"EXEC usp_GetOlder @MinionId";

            var dbConnectionString = @"Server=(localdb)\MSSQLLocalDB; Database=MinionsDB; Integrated Security=true";
            var dbConnection = new SqlConnection(dbConnectionString);
            dbConnection.Open();
            using(dbConnection)
            {
                var increaseAgeCommand = new SqlCommand(increaseAgeQuery, dbConnection);
                increaseAgeCommand.Parameters.AddWithValue("@MinionId", minionId);
                var dbReader = increaseAgeCommand.ExecuteReader();
                using(dbReader)
                {
                    while(dbReader.Read())
                    {
                        Console.WriteLine($"{(string)dbReader["Name"]} – {(int)dbReader["Age"]} years old");
                    }
                }
            }
        }
    }
}
