using Microsoft.Data.SqlClient;
using System;

namespace _06.RemoveVillain
{
    class Program
    {
        static void Main(string[] args)
        {
            var villainId = int.Parse(Console.ReadLine());
            var villainName = string.Empty;
            var minionsCount = 0;

            var dbConnectionString = "Server=(localdb)\\MSSQLLocalDB; Database=MinionsDB; Integrated Security=true";

            var getVillainNameQuery = @"SELECT Name FROM Villains WHERE Id = @villainId";
            var deleteMinionsVillainsQuery = @"DELETE FROM MinionsVillains WHERE VillainId = @villainId";
            var deleteVillainQuery = @"DELETE FROM Villains WHERE Id = @villainId";

            var dbConnection = new SqlConnection(dbConnectionString);
            dbConnection.Open();
            using (dbConnection)
            {
                var getVillainNameCommand = new SqlCommand(getVillainNameQuery, dbConnection);
                getVillainNameCommand.Parameters.AddWithValue("@villainId", villainId);

                villainName = (string)getVillainNameCommand.ExecuteScalar();

                if (villainName == null)
                {
                    Console.WriteLine("No such villain was found.");
                }
                else
                {
                    var deleteMinionsVillainsCommand = new SqlCommand(deleteMinionsVillainsQuery, dbConnection);
                    deleteMinionsVillainsCommand.Parameters.AddWithValue("@villainId", villainId);
                    minionsCount = deleteMinionsVillainsCommand.ExecuteNonQuery();

                    var deleteVillainCommand = new SqlCommand(deleteVillainQuery, dbConnection);
                    deleteVillainCommand.Parameters.AddWithValue("@villainId", villainId);
                    deleteVillainCommand.ExecuteNonQuery();

                    Console.WriteLine($"{villainName} was deleted.");
                    Console.WriteLine($"{minionsCount} minions were released.");
                }
            }
        }
    }
}
