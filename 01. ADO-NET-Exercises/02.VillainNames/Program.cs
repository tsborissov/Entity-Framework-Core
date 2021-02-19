using Microsoft.Data.SqlClient;
using System;
using System.Text;

namespace _02.VillainNames
{
    class Program
    {
        static void Main(string[] args)
        {
            string dbConnectionString = 
                "Server=(localdb)\\MSSQLLocalDB;" +
                "Database=MinionsDB;" +
                "Integrated Security=true";
            SqlConnection dbConnection = new SqlConnection(dbConnectionString);

            dbConnection.Open();

            using(dbConnection)
            {
                SqlCommand dbCommand = new SqlCommand(
                    @"SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
                        FROM Villains AS v 
                        JOIN MinionsVillains AS mv ON v.Id = mv.VillainId 
                    GROUP BY v.Id, v.Name 
                      HAVING COUNT(mv.VillainId) > 3 
                    ORDER BY COUNT(mv.VillainId)", dbConnection);

                SqlDataReader dbReader = dbCommand.ExecuteReader();

                using(dbReader)
                {
                    var result = new StringBuilder();

                    while(dbReader.Read())
                    {
                        string villainName = (string)dbReader["Name"];
                        int minionsCount = (int)dbReader["MinionsCount"];

                        result.AppendLine($"{villainName} - {minionsCount}");
                    }

                    Console.WriteLine(result.ToString());
                }
            }
        }
    }
}
