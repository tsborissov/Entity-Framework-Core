using Microsoft.Data.SqlClient;
using System;
using System.Linq;

namespace _04.AddMinion
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] minionInput = Console.ReadLine().Split().ToArray();
            string minionName = minionInput[1];
            int minionAge = int.Parse(minionInput[2]);
            string minionTown = minionInput[3];

            string[] villainInput = Console.ReadLine().Split().ToArray();
            string villainName = villainInput[1];

            string dbConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=MinionsDB; Integrated Security=true";
            SqlConnection dbConnection = new SqlConnection(dbConnectionString);
            dbConnection.Open();
            using (dbConnection)
            {
                SqlCommand getTownIdQuery = new SqlCommand("SELECT Id FROM Towns WHERE Name = @townName", dbConnection);
                getTownIdQuery.Parameters.AddWithValue("@townName", minionTown);

                var resultTownId = getTownIdQuery.ExecuteScalar();

                if (resultTownId == null)
                {
                    SqlCommand addTownQuery = new SqlCommand("INSERT INTO Towns(Name) VALUES(@townName)", dbConnection);
                    addTownQuery.Parameters.AddWithValue("@townName", minionTown);
                    addTownQuery.ExecuteNonQuery();

                    Console.WriteLine($"Town {minionTown} was added to the database.");
                }

                int townId = (int)getTownIdQuery.ExecuteScalar();


                SqlCommand getVillainIdQuery = new SqlCommand("SELECT Id FROM Villains WHERE Name = @Name", dbConnection);
                getVillainIdQuery.Parameters.AddWithValue("@Name", villainName);

                var resultVillainId = getVillainIdQuery.ExecuteScalar();

                if (resultVillainId == null)
                {
                    SqlCommand addVillainQuery = new SqlCommand("INSERT INTO Villains(Name, EvilnessFactorId)  VALUES(@villainName, 4)", dbConnection);
                    addVillainQuery.Parameters.AddWithValue("@villainName", villainName);
                    addVillainQuery.ExecuteNonQuery();

                    Console.WriteLine($"Villain {villainName} was added to the database.");
                }

                int villainId = (int)getVillainIdQuery.ExecuteScalar();


                SqlCommand addMinionQuery = new SqlCommand("INSERT INTO Minions(Name, Age, TownId) VALUES(@name, @age, @townId)", dbConnection);
                addMinionQuery.Parameters.AddWithValue("@name", minionName);
                addMinionQuery.Parameters.AddWithValue("@age", minionAge);
                addMinionQuery.Parameters.AddWithValue("@townId", townId);

                addMinionQuery.ExecuteNonQuery();

                SqlCommand getMinionId = new SqlCommand("SELECT Id FROM Minions WHERE Name = @Name", dbConnection);
                getMinionId.Parameters.AddWithValue("@Name", minionName);

                int minionId = (int)getMinionId.ExecuteScalar();

                SqlCommand linkMinionToVillain = new SqlCommand("INSERT INTO MinionsVillains(MinionId, VillainId) VALUES(@villainId, @minionId)", dbConnection);
                linkMinionToVillain.Parameters.AddWithValue("@villainId", villainId);
                linkMinionToVillain.Parameters.AddWithValue("@minionId", minionId);

                Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
            }
        }
    }
}
